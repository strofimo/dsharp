using System;
using System.Collections.Generic;
using System.Linq;
using DSharp.Compiler.ScriptModel.Expressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace DSharp.Compiler.Preprocessing.Lowering
{
    public class ExtensionMethodToStaticRewriter : CSharpSyntaxRewriter, ILowerer
    {
        private SemanticModel sem;
        private Dictionary<SyntaxNode, SyntaxNode> trackedNodes = new Dictionary<SyntaxNode, SyntaxNode>();

        public CompilationUnitSyntax Apply(Compilation compilation, CompilationUnitSyntax root)
        {
            sem = compilation.GetSemanticModel(root.SyntaxTree);
            var newRoot = Visit(root) as CompilationUnitSyntax;
            return newRoot;
        }

        public override SyntaxNode VisitInvocationExpression(InvocationExpressionSyntax node)
        {
            var symb = Try(() => sem.GetSymbolInfo(node).Symbol as IMethodSymbol, null);
            var newNode = (InvocationExpressionSyntax)base.VisitInvocationExpression(node);

            if (symb != null && symb.IsExtensionMethod)
            {
                var extensionClassName = symb.ContainingSymbol.Name;
                
                switch (newNode.Expression)
                {
                    case MemberAccessExpressionSyntax memberExpression:
                    {
                        var newArguments = ArgumentList();
                        newArguments = newArguments.AddArguments(Argument(memberExpression.Expression.WithoutTrivia())); // extension method target
                        newArguments = newArguments.AddArguments(newNode.ArgumentList.Arguments.ToArray());

                        return InvocationExpression(
                            MemberAccessExpression(
                                memberExpression.Kind(),
                                IdentifierName(extensionClassName),
                                memberExpression.OperatorToken,
                                memberExpression.Name),
                            newArguments);
                    }

                    case SimpleNameSyntax nameExpression:
                    {
                       return InvocationExpression(
                            MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                IdentifierName(extensionClassName),
                                nameExpression.WithoutTrivia()),
                            newNode.ArgumentList);
                    }

                    default:
                        throw new NotImplementedException(); // please implement extra cases!
                }
            }

            return newNode;
        }

        private static T Try<T>(Func<T> action, T defaultValue)
        {
            try { return action(); }
            catch (Exception ex) { throw ex; }

            return defaultValue;
        }
    }
}
