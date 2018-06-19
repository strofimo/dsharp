using System;

namespace DSharp.Compiler.Errors
{
    public class ExceptionError : IError
    {
        private readonly string tokenLocation;

        public ExceptionError(Exception exception, string tokenLocation)
        {
            Exception = exception;
            this.tokenLocation = tokenLocation;
        }

        public string Message => Exception.Message;

        public string Location => Exception.StackTrace + Environment.NewLine + $"Token: {tokenLocation}";

        public Exception Exception { get; }
    }
}
