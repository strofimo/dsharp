namespace DSharp.Compiler.TestFramework
{
    public interface IError
    {
        string Message { get; }

        string Location { get; }
    }
}
