namespace DSharp.Compiler.TestFramework.Compilation
{
    public interface IError
    {
        string Message { get; }

        string Location { get; }
    }
}
