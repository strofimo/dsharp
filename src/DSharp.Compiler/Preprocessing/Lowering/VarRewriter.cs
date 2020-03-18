using System;
using System.Collections.Generic;
using System.Linq;
using DSharp.Compiler.Errors;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace DSharp.Compiler.Preprocessing.Lowering
{
    public class VarRewriter : CSharpSyntaxRewriter, ILowerer
    {
        private readonly IErrorHandler errorHandler;
        private static readonly SymbolDisplayFormat displayFormat = new SymbolDisplayFormat(
                            genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters,
                            typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypes,
                            miscellaneousOptions: SymbolDisplayMiscellaneousOptions.ExpandNullable | SymbolDisplayMiscellaneousOptions.UseSpecialTypes
                        );

        private SemanticModel sem;
        private Dictionary<string, string> aliases;
        private HashSet<string> requiredUsings;

        public VarRewriter(IErrorHandler errorHandler)
        {
            this.errorHandler = errorHandler;
        }

        public CompilationUnitSyntax Apply(Compilation compilation, CompilationUnitSyntax root)
        {
            sem = compilation.GetSemanticModel(root.SyntaxTree);
            requiredUsings = new HashSet<string>();
            aliases = root.Usings
                .Where(u => u.Alias is NameEqualsSyntax)
                .ToDictionary(
                    keySelector: a => a.Name.ToString(),
                    elementSelector: a => a.Alias.ToString()
                );

            var newRoot = Visit(root) as CompilationUnitSyntax;

            var missingUsings = requiredUsings.Where(r => !root.Usings.Select(u => u.Name.ToString()).Contains(r));

            if (missingUsings.Any())
            {
                var missingDirectives = missingUsings.Select(s => UsingDirective(ParseName(s).WithLeadingTrivia(Whitespace(" ")))).ToArray();
                newRoot = newRoot.AddUsings(missingDirectives);
            }

            return newRoot;
        }

        public override SyntaxNode VisitVariableDeclaration(VariableDeclarationSyntax node)
        {
            if (node.Type.IsVar)
            {
                string typeName;

                if (sem.GetTypeInfo(node.Type).Type is INamedTypeSymbol type)
                {
                    if (!aliases.TryGetValue(type.ToString(), out typeName))
                    {
                        typeName = type.ToDisplayString(displayFormat);

                        requiredUsings.Add(type.ContainingNamespace.Name);

                        if (type.IsGenericType)
                        {
                            foreach (var namespaceName in type.TypeParameters.Select(t => t.ContainingNamespace.Name))
                            {
                                requiredUsings.Add(namespaceName);
                            }
                        }
                    }
                }
                else if(sem.GetTypeInfo(node.Type).Type is IArrayTypeSymbol arrayType)
                {
                    typeName = arrayType.ToDisplayString(displayFormat);
                }
                else
                {
                    errorHandler.ReportExpressionError($"unable to determine type of var: '{node.ToString()}'", node);
                    return base.VisitVariableDeclaration(node);
                }

                return node.WithType(ParseTypeName(typeName).WithTrailingTrivia(Whitespace(" ")).WithTriviaFrom(node.Type));
            }

            return base.VisitVariableDeclaration(node);
        }
    }
}
