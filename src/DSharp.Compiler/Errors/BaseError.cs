namespace DSharp.Compiler.Errors
{
    public abstract class BaseError : IError
    {
        public BaseError(string message, string location)
        {
            Message = message;
            Location = location;
        }

        public string Message { get; }

        public string Location { get; }
    }
}
