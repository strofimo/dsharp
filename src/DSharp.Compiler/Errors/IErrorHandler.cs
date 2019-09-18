namespace DSharp.Compiler.Errors
{
    public interface IErrorHandler
    {
        bool HasErrored { get; }

        void ReportError(CompilerError error);
    }
}
