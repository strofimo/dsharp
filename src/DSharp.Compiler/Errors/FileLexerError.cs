using DSharp.Compiler.Parser;

namespace DSharp.Compiler.Errors
{
    internal class FileLexerError : BaseError
    {
        public FileLexerError(string filePath, FileErrorEventArgs fileErrorEventArgs)
            : base(string.Format(fileErrorEventArgs.Error.Message, fileErrorEventArgs.Args), fileErrorEventArgs.Position.ToString())
        {
            FilePath = filePath;
            FileErrorEventArgs = fileErrorEventArgs;
        }

        public string FilePath { get; }

        public FileErrorEventArgs FileErrorEventArgs { get; }
    }
}
