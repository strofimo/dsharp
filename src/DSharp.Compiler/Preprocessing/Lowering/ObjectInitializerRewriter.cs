using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace DSharp.Compiler.Preprocessing.Lowering
{
    public class ObjectInitializerRewriter : CSharpSyntaxRewriter, ILowerer
    {
        public CompilationUnitSyntax Apply(Compilation compilation, CompilationUnitSyntax root)
        {
            var newRoot = Visit(root) as CompilationUnitSyntax;

            if(!newRoot.Usings.Any(u=>u.Name.ToString() == "System"))
            {
                newRoot = newRoot.AddUsings(UsingDirective(ParseName("System").WithLeadingTrivia(Space)));
            }

            return newRoot;
        }

        public override SyntaxNode VisitObjectCreationExpression(ObjectCreationExpressionSyntax node)
        {
            if (node.Initializer == null)
            {
                return base.VisitObjectCreationExpression(node);
            }

            ObjectCreationExpressionSyntax newNode = (ObjectCreationExpressionSyntax)base.VisitObjectCreationExpression(node);

            var openBrace = newNode.Initializer.OpenBraceToken;
            var closeBrace = newNode.Initializer.CloseBraceToken;

            IdentifierNameSyntax obj = IdentifierName("_obj_");
            var constructObject = GenerateObject(newNode.Type.WithoutTrailingTrivia(), obj.Identifier)
                .WithTrailingTrivia(openBrace.TrailingTrivia)
                .WithLeadingTrivia((newNode.ArgumentList?.GetTrailingTrivia() ?? newNode.Type.GetTrailingTrivia()).AddRange(openBrace.LeadingTrivia));

            var initialiseProperties = newNode.Initializer.Expressions
                .Cast<AssignmentExpressionSyntax>()
                .Select((e, i) => ProcessPropertyExpression(obj, e, GetTrailingPropertyTrivia(i, node.Initializer.Expressions)))
                .ToArray();
            
            var returnObject = ReturnStatement(obj.WithLeadingTrivia(Space)).WithLeadingTrivia(closeBrace.LeadingTrivia);

            var func = GenerateFunction(constructObject, initialiseProperties, returnObject);

            return GenerateInvocation(newNode, func);
        }

        private static AnonymousMethodExpressionSyntax GenerateFunction(LocalDeclarationStatementSyntax constructObject, StatementSyntax[] initialiseProperties, ReturnStatementSyntax returnObject)
        {
            return AnonymousMethodExpression(
                body: Block(
                    openBraceToken: Token(SyntaxKind.OpenBraceToken),
                    statements: SingletonList<StatementSyntax>(constructObject),
                    closeBraceToken: Token(SyntaxKind.CloseBraceToken))
                    .AddStatements(initialiseProperties)
                    .AddStatements(returnObject)
            );
        }

        private static SyntaxNode GenerateInvocation(ObjectCreationExpressionSyntax newNode, AnonymousMethodExpressionSyntax func)
        {
            return InvocationExpression(
                expression: MemberAccessExpression(
                    kind: SyntaxKind.SimpleMemberAccessExpression,
                    expression: ParenthesizedExpression(CastExpression(
                            type: ParseTypeName($"Func<{newNode.Type}>"),
                            expression: func
                        )
                    ),
                    name: IdentifierName(nameof(Action.Invoke))
                )
            );
        }

        private static SyntaxTriviaList GetTrailingPropertyTrivia(int i, SeparatedSyntaxList<ExpressionSyntax> expressions)
        {
            return expressions.SeparatorCount > i ? expressions.GetSeparator(i).TrailingTrivia : expressions[i].GetTrailingTrivia();
        }

        private static LocalDeclarationStatementSyntax GenerateObject(TypeSyntax type, SyntaxToken objectIdentifier)
        {
            return LocalDeclarationStatement(
                            VariableDeclaration(type)
                            .WithVariables(
                                SingletonSeparatedList(
                                    VariableDeclarator(objectIdentifier.WithLeadingTrivia(Space))
                                    .WithInitializer(
                                        EqualsValueClause(
                                            ObjectCreationExpression(type.WithLeadingTrivia(Space))
                                            .WithArgumentList(ArgumentList())
                                        )
                                    )
                                )
                            )
                        );
        }

        public StatementSyntax ProcessPropertyExpression(IdentifierNameSyntax obj, AssignmentExpressionSyntax assignmentExpression, SyntaxTriviaList trailingTrivia)
        {
            var expression = assignmentExpression.WithLeft(
                MemberAccessExpression(
                    kind: SyntaxKind.SimpleMemberAccessExpression,
                    expression: obj,
                    name: (IdentifierNameSyntax)assignmentExpression.Left.WithoutLeadingTrivia()
                )
            ).WithLeadingTrivia(assignmentExpression.GetLeadingTrivia());

            return ExpressionStatement(expression).WithTrailingTrivia(trailingTrivia);
        }
    }
}
