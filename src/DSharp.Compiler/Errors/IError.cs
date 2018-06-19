namespace DSharp.Compiler.Errors
{
    public interface IError
    {
        string Message { get; }

        string Location { get; }
    }
}
