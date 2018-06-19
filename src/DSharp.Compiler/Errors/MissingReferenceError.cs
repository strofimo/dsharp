namespace DSharp.Compiler.Errors
{
    public class MissingReferenceError : BaseError
    {
        public MissingReferenceError(string message)
            : base(message, string.Empty)
        {
        }

        public MissingReferenceError(string message, string path)
            : base(message, path)
        {
        }
    }
}
