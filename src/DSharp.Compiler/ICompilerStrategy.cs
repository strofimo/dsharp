using DSharp.Compiler.Errors;

namespace DSharp.Compiler
{
    public interface ICompilerStrategy
    {
        void Compile(CompilerOptions options, IErrorHandler errorHandler);
    }
}
