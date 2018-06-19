namespace DSharp.Compiler.Errors
{
    public class InputFileError : IError
    {
        public InputFileError(string filePath)
        {
            FilePath = filePath;
        }

        public string Message => $"Unable to load file {FilePath}";

        public string FilePath;

        public string Location => FilePath;
    }
}
