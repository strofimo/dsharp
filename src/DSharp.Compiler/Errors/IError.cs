namespace DSharp.Compiler.Errors
{
    //TODO: Look at making a uniform object with the following; Error code, File, Line number, Message.
    //TODO: Remove exception logic from errors and push down to throw from the compiler object as an exceptional issue
    public interface IError
    {
        string Message { get; }

        string Location { get; }
    }
}
