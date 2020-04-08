using System.Linq;
using DSharp.Compiler.Errors;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;

using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace DSharp.Compiler.Preprocessing.Lowering
{
    internal class OperatorOverloadRewriter : CSharpSyntaxRewriter, ILowerer
    {
        private SemanticModel sem;
        private static readonly SymbolDisplayFormat displayFormat = new SymbolDisplayFormat(
                    genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters,
                    typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypes,
                    miscellaneousOptions: SymbolDisplayMiscellaneousOptions.ExpandNullable | SymbolDisplayMiscellaneousOptions.UseSpecialTypes
                );

        public CompilationUnitSyntax Apply(Compilation compilation, CompilationUnitSyntax root)
        {
            sem = compilation.GetSemanticModel(root.SyntaxTree);
            var newRoot = Visit(root) as CompilationUnitSyntax;
            return newRoot;
        }

        public override SyntaxNode VisitOperatorDeclaration(OperatorDeclarationSyntax node)
        {
            var symbol = sem.GetDeclaredSymbol(node);
            var newNode = (OperatorDeclarationSyntax)base.VisitOperatorDeclaration(node);

            if (symbol.GetAttributes().Any(a => a.AttributeClass.Name.StartsWith("ScriptIgnore")))
            {
                return newNode;
            }

            return MethodDeclaration(
                attributeLists: newNode.AttributeLists,
                modifiers: newNode.Modifiers,
                returnType: newNode.ReturnType,
                identifier: Identifier(symbol.Name)
                    .WithLeadingTrivia(newNode.OperatorToken.LeadingTrivia)
                    .WithTrailingTrivia(newNode.OperatorToken.TrailingTrivia),
                parameterList:newNode.ParameterList,
                body: newNode.Body,
                typeParameterList: default,
                constraintClauses: default,
                explicitInterfaceSpecifier: default,
                expressionBody:default
            );
        }

        public override SyntaxNode VisitBinaryExpression(BinaryExpressionSyntax node)
        {
            var newNode = base.VisitBinaryExpression(node);
            if (sem.GetOperation(node) is IBinaryOperation operation && operation.OperatorMethod is IMethodSymbol operatorMethod)
            {
                if(operatorMethod.GetAttributes().Any(a=>a.AttributeClass.Name.StartsWith("ScriptIgnore")))
                {
                    return newNode;
                }

                var identifier = IdentifierName(operatorMethod.Name);
                var type = ParseTypeName(operatorMethod.ContainingType.ToDisplayString(displayFormat));
                return InvocationExpression(
                    expression: MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, type, identifier),
                    argumentList: ArgumentList(Token(SyntaxKind.OpenParenToken), SeparatedList(new [] { Argument(node.Left), Argument(node.Right) }), Token(SyntaxKind.CloseParenToken))
                );
            }
            return newNode;
        }

        public override SyntaxNode VisitPostfixUnaryExpression(PostfixUnaryExpressionSyntax node)
        {
            var newNode = base.VisitPostfixUnaryExpression(node);
            if (sem.GetOperation(node) is IUnaryOperation operation && operation.OperatorMethod is IMethodSymbol operatorMethod)
            {
                if (operatorMethod.GetAttributes().Any(a => a.AttributeClass.Name.StartsWith("ScriptIgnore")))
                {
                    return newNode;
                }

                var identifier = IdentifierName(operatorMethod.Name);
                var type = ParseTypeName(operatorMethod.ContainingType.ToDisplayString(displayFormat));
                return InvocationExpression(
                    expression: MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, type, identifier),
                    argumentList: ArgumentList(Token(SyntaxKind.OpenParenToken), SeparatedList(new[] { Argument(node.Operand) }), Token(SyntaxKind.CloseParenToken))
                );
            }
            return newNode;
        }

        public override SyntaxNode VisitPrefixUnaryExpression(PrefixUnaryExpressionSyntax node)
        {
            var newNode = base.VisitPrefixUnaryExpression(node);
            if (sem.GetOperation(node) is IUnaryOperation operation && operation.OperatorMethod is IMethodSymbol operatorMethod)
            {
                if (operatorMethod.GetAttributes().Any(a => a.AttributeClass.Name.StartsWith("ScriptIgnore")))
                {
                    return newNode;
                }

                var identifier = IdentifierName(operatorMethod.Name);
                var type = ParseTypeName(operatorMethod.ContainingType.ToDisplayString(displayFormat));
                return InvocationExpression(
                    expression: MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, type, identifier),
                    argumentList: ArgumentList(Token(SyntaxKind.OpenParenToken), SeparatedList(new[] { Argument(node.Operand) }), Token(SyntaxKind.CloseParenToken))
                );
            }
            return newNode;
        }
    }
}
