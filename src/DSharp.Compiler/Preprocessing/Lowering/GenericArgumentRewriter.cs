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
    public class GenericArgumentRewriter : CSharpSyntaxRewriter, ILowerer
    {
        private readonly IErrorHandler errorHandler;
        private SemanticModel semanticModel;
        private Dictionary<string, string> aliases;
        private HashSet<string> requiredUsings;

        public GenericArgumentRewriter(IErrorHandler errorHandler)
        {
            this.errorHandler = errorHandler;
        }

        public CompilationUnitSyntax Apply(Compilation compilation, CompilationUnitSyntax root)
        {
            semanticModel = compilation.GetSemanticModel(root.SyntaxTree);
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
        
        public override SyntaxNode VisitIdentifierName(IdentifierNameSyntax node)
        {
            var symbol = semanticModel.GetSymbolInfo(node).Symbol;

            if(symbol is IMethodSymbol methodSymbol)
            {
                return VisitMethodSymbol(methodSymbol, node);
            }

            return base.VisitIdentifierName(node);
        }

        private SyntaxNode VisitMethodSymbol(IMethodSymbol methodSymbol, IdentifierNameSyntax node)
        {
            if(methodSymbol.Arity != node.Arity)
            {
                return ParseName(methodSymbol.ToDisplayString(new SymbolDisplayFormat(genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters))).WithTriviaFrom(node);

            }

            return base.VisitIdentifierName(node);
        }
    }
}
