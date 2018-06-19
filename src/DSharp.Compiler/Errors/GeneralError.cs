namespace DSharp.Compiler.Errors
{
    public class GeneralError : BaseError
    {
        public GeneralError(string errorMessage) 
            : base(errorMessage, string.Empty)
        {
        }
    }
}
