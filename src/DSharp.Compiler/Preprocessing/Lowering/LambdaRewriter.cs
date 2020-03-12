using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace DSharp.Compiler.Preprocessing.Lowering
{
    public class LambdaRewriter : CSharpSyntaxRewriter, ILowerer
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
