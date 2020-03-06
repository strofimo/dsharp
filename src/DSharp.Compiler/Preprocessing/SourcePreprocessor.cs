using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace DSharp.Compiler.Preprocessing
{
    internal class SourcePreprocessor
    {
        internal IStreamSource Preprocess(IStreamSource source)
        {
            using (Stream stream = source.GetStream())
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    return new StringSource()
                    {
                        FullName = source.FullName,
                        Name = source.Name,
                        Contents = Preprocess(stream, source.FullName)
                    };
                }
            }
        }

        private string Preprocess(Stream stream, string path)
        {
            var text = SourceText.From(stream);
            var syntaxTree = CSharpSyntaxTree.ParseText(text, path: path);
            CSharpCompilation comp = CSharpCompilation.Create("test", new[] { syntaxTree }, new[] { MetadataReference.CreateFromFile(typeof(object).Assembly.Location) });
            var sem = comp.GetSemanticModel(syntaxTree);
            var newRoot = new LambdaRewriter(sem).Visit(syntaxTree.GetRoot());
            return newRoot.ToFullString();
        }
    }

    class LambdaRewriter : CSharpSyntaxRewriter
    {
        private SemanticModel sem;

        public LambdaRewriter(SemanticModel sem)
        {
            this.sem = sem;
        }

        public override SyntaxNode VisitSimpleLambdaExpression(SimpleLambdaExpressionSyntax node)
        {
            var symb = sem.GetSymbolInfo(node).Symbol as IMethodSymbol;
            return AnonymousMethodExpression(
                parameterList: ParameterList(SingletonSeparatedList(FormatParam(node.Parameter,0,symb))),
                 body: node.Body is ExpressionSyntax expression
                    ? Block(ReturnStatement(expression.WithLeadingTrivia(Whitespace(" "))))
                    : node.Body as StatementSyntax
            );
        }

        public override SyntaxNode VisitParenthesizedLambdaExpression(ParenthesizedLambdaExpressionSyntax node)
        {
            var symb = sem.GetSymbolInfo(node).Symbol as IMethodSymbol;
            var paramSyntax = node.ParameterList.Parameters.Select((p, i) => FormatParam(p, i, symb));
            return AnonymousMethodExpression(
                parameterList: node.ParameterList.WithParameters(SeparatedList(paramSyntax)),
                body: node.Body is ExpressionSyntax expression
                    ? Block(ReturnStatement(expression.WithLeadingTrivia(Whitespace(" "))))
                    : node.Body as StatementSyntax
            );
        }

        private static ParameterSyntax FormatParam(ParameterSyntax p, int i, IMethodSymbol symb)
        {
            return p.WithType(ParseTypeName(symb.Parameters[i].Type.Name).WithTrailingTrivia(Whitespace(" ")));
        }
    }
}
