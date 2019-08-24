using DSharp.Compiler.CodeModel.Tokens;

namespace DSharp.Compiler.CodeModel.Expressions
{
    internal class ParameterizedExpressionNode : ExpressionNode
    {
        public ParameterizedExpressionNode(Token token, ParseNode innerExpression)
            : base(ParseNodeType.ParameterizedExpression, token)
        {
            InnerExpression = innerExpression;
        }

        public ParseNode InnerExpression { get; }
    }
}
