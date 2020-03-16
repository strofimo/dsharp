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
                var type = sem.GetTypeInfo(node.Type).Type as INamedTypeSymbol;

                if(type is null)
                {
                    errorHandler.ReportExpressionError($"unable to determine type of var: '{node.ToString()}'", node);
                    return base.VisitVariableDeclaration(node);
                }

                string typeName;

                if (!aliases.TryGetValue(type.ToString(), out typeName))
                {
                    typeName = type.ToDisplayString(new SymbolDisplayFormat(
                        genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters,
                        typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypes,
                        miscellaneousOptions: SymbolDisplayMiscellaneousOptions.ExpandNullable | SymbolDisplayMiscellaneousOptions.UseSpecialTypes
                    ));

                    requiredUsings.Add(type.ContainingNamespace.Name);

                    if (type.IsGenericType)
                    {
                        foreach (var namespaceName in type.TypeParameters.Select(t => t.ContainingNamespace.Name))
                        {
                            requiredUsings.Add(namespaceName);
                        }
                    }
                }

                return node.WithType(ParseTypeName(typeName).WithTrailingTrivia(Whitespace(" ")).WithTriviaFrom(node.Type));
            }

            return base.VisitVariableDeclaration(node);
        }
    }
}
