namespace DSharp.Compiler.TestFramework
{
    public interface ICompilationUnitResult
    {
        string Output { get; }

        IError[] Errors { get; }
    }
}
