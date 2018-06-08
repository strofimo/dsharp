namespace DSharp.Compiler.TestFramework.Compilation
{
    public interface ICompilationUnitResult
    {
        string Output { get; }

        IError[] Errors { get; }
    }
}
