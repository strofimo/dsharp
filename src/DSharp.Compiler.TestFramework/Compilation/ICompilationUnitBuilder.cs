using ScriptSharp;

namespace DSharp.Compiler.TestFramework
{
    public interface ICompilationUnitBuilder
    {
        CompilerOptions Options { get; }

        ICompilationUnit Build();
    }
}
