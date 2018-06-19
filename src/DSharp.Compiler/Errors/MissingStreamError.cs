namespace DSharp.Compiler.Errors
{
    public class MissingStreamError : BaseError
    {
        public MissingStreamError(string message, string location) 
            : base(message, location)
        {
        }
    }
}
