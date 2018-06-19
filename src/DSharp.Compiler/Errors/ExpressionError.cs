namespace DSharp.Compiler.Errors
{
    public class ExpressionError : BaseError
    {
        public ExpressionError(string message, string location)
            : base(message, location)
        {
        }
    }
}
