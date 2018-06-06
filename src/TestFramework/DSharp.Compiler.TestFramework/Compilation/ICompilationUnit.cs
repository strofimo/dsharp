namespace DSharp.Compiler.TestFramework
{
    public interface ICompilationUnit
    {
        bool Compile(out ICompilationUnitResult result);
    }
}
