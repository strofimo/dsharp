// ScriptGenerator.cs
// Script#/Core/Compiler
// This source code is subject to terms and conditions of the Apache License, Version 2.0.
//

using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using DSharp.Compiler.ScriptModel.Symbols;

namespace DSharp.Compiler.Generator
{
    internal sealed class ScriptMetadataGenerator
    {
        private readonly IComparer<TypeSymbol> typeComparer = new ScriptGenerator.TypeComparer();

        private SymbolSet symbols;

        public ScriptMetadataGenerator(TextWriter writer, CompilerOptions options, SymbolSet symbols)
        {
            Debug.Assert(writer != null);
            Writer = new ScriptTextWriter(writer);

            Options = options;
            this.symbols = symbols;
        }

        public CompilerOptions Options { get; }

        public ScriptTextWriter Writer { get; }

        public void GenerateScriptMetadata(SymbolSet symbolSet)
        {
            Debug.Assert(symbolSet != null);

            var types = CollectEmittableTypes(symbolSet)
                .OrderBy(t => t, typeComparer);

            Writer.Write("(function(");
            Writer.Write(string.Join(",", symbols.Dependencies.Select(d=>d.Identifier)));
            Writer.WriteLine(") {");
            Writer.Indent++;
            Writer.WriteLine("\"use strict\"");
            Writer.WriteLine("var Void = null;");
            Writer.WriteLine($"var module = ss.modules['{symbols.ScriptName}'];");

            foreach (TypeSymbol type in types)
            {
                if (type.Members?.Where(MemberHasMetadata) is IEnumerable<MemberSymbol> members && members.Any())
                {
                    Writer.WriteLine($"module.{type.GeneratedName}.$members = [");
                    Writer.Indent++;
                    WriteMembers(members);
                    Writer.Indent--;
                    Writer.WriteLine("];");
                }
            }

            Writer.Indent--;
            Writer.Write("})(");
            Writer.Write(string.Join(",", symbols.Dependencies.Select(d=>d.Identifier)));
            Writer.Write(");");
        }

        private void WriteMembers(IEnumerable<MemberSymbol> members)
        {
            bool first = true;
            foreach (var member in members)
            {
                if (!first)
                {
                    Writer.WriteLine(",");
                }
                first = false;
                
                if (member.Type == SymbolType.Method)
                {
                    WriteMethod((MethodSymbol)member);
                }

                if (member.Type == SymbolType.Property)
                {
                    WriteProperty((PropertySymbol)member);
                }

                if (member.Type == SymbolType.Field)
                {
                    WriteField((FieldSymbol)member);
                }
            }
        }

        private void WriteMethod(MethodSymbol member)
        {
            Writer.Write("{");
            Writer.Write("MemberType: 8,");
            Writer.Write($"Name: '{member.GeneratedName}'");
            Writer.Write(", ");
            Writer.Write("Type: ");
            if(member.AssociatedType.IsApplicationType)
            {
                Writer.Write("module.");
            }
            Writer.Write(member.AssociatedType.FullGeneratedName);
            Writer.Write("}");
        }

        private void WriteProperty(PropertySymbol member)
        {
            Writer.Write("{");
            Writer.Write("MemberType: 16,");
            Writer.Write($"Name: '{member.GeneratedName}'");
            Writer.Write(", ");
            Writer.Write("Type: ");
            if (member.AssociatedType.IsApplicationType)
            {
                Writer.Write("module.");
            }
            Writer.Write(member.AssociatedType.FullGeneratedName);
            Writer.Write("}");
        }

        private void WriteField(FieldSymbol member)
        {
            Writer.Write("{");
            Writer.Write("MemberType: 4,");
            Writer.Write($"Name: '{member.GeneratedName}'");
            Writer.Write(", ");
            Writer.Write("Type: ");
            if (member.AssociatedType.IsApplicationType)
            {
                Writer.Write("module.");
            }
            Writer.Write(member.AssociatedType.FullGeneratedName);
            Writer.Write("}");
        }

        private bool MemberHasMetadata(MemberSymbol arg)
        {
            switch (arg.Type)
            {
                case SymbolType.Method:
                case SymbolType.Field:
                case SymbolType.Property:
                //case SymbolType.Indexer: //todo
                //case SymbolType.Event: //todo
                    return true;
                default:
                    return false;
            }
        }

        private static List<TypeSymbol> CollectEmittableTypes(SymbolSet symbolSet)
        {
            var types = new List<TypeSymbol>();

            foreach (NamespaceSymbol namespaceSymbol in symbolSet.Namespaces.Where(ns => ns.HasApplicationTypes))
                foreach (TypeSymbol type in namespaceSymbol.Types.Where(type => type.IsApplicationType))
                {
                    switch (type.Type)
                    {
                        case SymbolType.Interface:
                        case SymbolType.Class:
                            types.Add(type);
                            break;

                        default:
                            break;
                    }
                }

            return types;
        }
    }
}
