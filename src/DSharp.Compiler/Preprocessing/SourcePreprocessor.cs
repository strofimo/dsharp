using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace DSharp.Compiler.Preprocessing
{
    internal class SourcePreprocessor
    {
        //internal IStreamSource Preprocess(CSharpCompilation comp, IStreamSource source)
        //{
        //    return new StringSource()
        //    {
        //        FullName = source.FullName,
        //        Name = source.Name,
        //        Contents = Preprocess(comp, source.FullName)
        //    };
        //}

        public CSharpCompilation Preprocess(CSharpCompilation comp)
        {
            var lowerers = new ILowerer[] {
                new StaticUsingRewriter(),
                new VarRewriter(),
                new LambdaRewriter()
            };

            for (int i = 0; i < comp.SyntaxTrees.Length; ++i)
            {
                foreach (var lowerer in lowerers)
                {
                    var syntaxTree = comp.SyntaxTrees[i];
                    var newRoot = lowerer.Apply(comp, syntaxTree.GetCompilationUnitRoot());
                    comp = comp.ReplaceSyntaxTree(syntaxTree, syntaxTree.WithRootAndOptions(newRoot, syntaxTree.Options));
                }
            }

            return comp;

            //var syntaxTree = comp.SyntaxTrees.Single(p => Path.GetFullPath(path).Equals(Path.GetFullPath(p.FilePath), StringComparison.OrdinalIgnoreCase));



            //comp = comp.ReplaceSyntaxTree(syntaxTree, syntaxTree.WithRootAndOptions(new StaticUsingRewriter().Apply(comp, syntaxTree.GetCompilationUnitRoot()), syntaxTree.Options));



            //var sem = comp.GetSemanticModel(syntaxTree);
            //comp = comp.ReplaceSyntaxTree(syntaxTree, .synta
            //var newRoot = syntaxTree.GetRoot() as CompilationUnitSyntax;
            //newRoot = new StaticUsingRewriter(sem).Apply(newRoot);
            //newRoot = new VarRewriter(sem).Apply(newRoot);
            //newRoot = new LambdaRewriter(sem).Apply(newRoot);
            //return newRoot.ToFullString();
        }
    }

    public interface ILowerer
    {
        CompilationUnitSyntax Apply(Compilation compilation, CompilationUnitSyntax root);
    }

    class StaticUsingRewriter : CSharpSyntaxRewriter, ILowerer
    {
        private SemanticModel sem;
        private IEnumerable<ITypeSymbol> staticUsingTypes;

        public CompilationUnitSyntax Apply(Compilation compilation, CompilationUnitSyntax root)
        {
            sem = compilation.GetSemanticModel(root.SyntaxTree);
            var staticUsings = root.Usings.Where(IsStatic);
            staticUsingTypes = staticUsings.Select(GetType);

            var newRoot = Visit(root) as CompilationUnitSyntax;

            return newRoot;
        }

        private bool IsStatic(UsingDirectiveSyntax node)
        {
            return node.StaticKeyword != default;
        }

        private ITypeSymbol GetType(UsingDirectiveSyntax node)
        {
            return sem.GetTypeInfo(node.Name).Type;
        }

        public override SyntaxNode VisitUsingDirective(UsingDirectiveSyntax node)
        {
            if (IsStatic(node))
            {
                string namespaceName = GetType(node).ContainingNamespace?.Name;

                return UsingDirective(ParseName(namespaceName).WithLeadingTrivia(Whitespace(" "))).WithTriviaFrom(node);
            }

            return base.VisitUsingDirective(node);
        }

        public override SyntaxNode VisitIdentifierName(IdentifierNameSyntax node)
        {
            if (node.Parent is MemberAccessExpressionSyntax)
            {
                //not static using
                return base.VisitIdentifierName(node);
            }

            var symbol = sem.GetSymbolInfo(node).Symbol as ISymbol;
            var type = symbol?.ContainingType;
            if (staticUsingTypes.Contains(type))
            {
                switch (symbol)
                {
                    case IFieldSymbol _:
                    case IMethodSymbol _:
                    case IPropertySymbol _:
                    case IEventSymbol _:
                        return MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, ParseTypeName(type.Name), node.WithoutTrivia()).WithTriviaFrom(node);
                }
            }

            return base.VisitIdentifierName(node);
        }
    }

    class VarRewriter : CSharpSyntaxRewriter, ILowerer
    {
        private SemanticModel sem;
        private Dictionary<string, string> aliases;
        private HashSet<string> requiredUsings;

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

    class LambdaRewriter : CSharpSyntaxRewriter, ILowerer
    {
        private SemanticModel sem;

        public CompilationUnitSyntax Apply(Compilation compilation, CompilationUnitSyntax root)
        {
            sem = compilation.GetSemanticModel(root.SyntaxTree);
            return Visit(root) as CompilationUnitSyntax;
        }

        public override SyntaxNode VisitSimpleLambdaExpression(SimpleLambdaExpressionSyntax node)
        {
            var symb = sem.GetSymbolInfo(node).Symbol as IMethodSymbol;
            return AnonymousMethodExpression(
                parameterList: ParameterList(SingletonSeparatedList(FormatParam(node.Parameter, 0, symb))),
                 body: GetFunctionBody(node.Body)
            );
        }

        public override SyntaxNode VisitParenthesizedLambdaExpression(ParenthesizedLambdaExpressionSyntax node)
        {
            var symb = sem.GetSymbolInfo(node).Symbol as IMethodSymbol;
            var paramSyntax = node.ParameterList.Parameters.Select((p, i) => FormatParam(p, i, symb));
            return AnonymousMethodExpression(
                parameterList: node.ParameterList.WithParameters(SeparatedList(paramSyntax)),
                body: GetFunctionBody(node.Body)
            );
        }

        private CSharpSyntaxNode GetFunctionBody(SyntaxNode node)
        {
            if (node is ExpressionSyntax expression)
            {
                if (ExpressionHasType(expression))
                {
                    return Block(ReturnStatement(expression.WithLeadingTrivia(Whitespace(" "))));
                }

                return Block(ExpressionStatement(expression));
            }

            if (node is BlockSyntax block)
            {
                return block;
            }

            if (node is StatementSyntax statement)
            {
                return Block(statement);
            }

            //todo: report error
            return null;
        }

        private bool ExpressionHasType(ExpressionSyntax expression)
        {
            return sem.GetTypeInfo(expression).Type is ITypeSymbol ts && ts.Kind != SymbolKind.ErrorType;
        }

        private static ParameterSyntax FormatParam(ParameterSyntax p, int i, IMethodSymbol symb)
        {
            return p.WithType(ParseTypeName(symb.Parameters[i].Type.Name).WithTrailingTrivia(Whitespace(" ")));
        }
    }
}
