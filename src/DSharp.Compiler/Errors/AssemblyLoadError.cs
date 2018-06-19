namespace DSharp.Compiler.Errors
{
    public class AssemblyLoadError : BaseError
    {
        public AssemblyLoadError(string assemblyName, string path)
            : base($"The referenced assembly '{assemblyName}' could not be loaded as an assembly.", string.Empty)
        {
            AssemblyName = assemblyName;
            Path = path;
        }

        public string AssemblyName { get; }

        public string Path { get; }
    }
}
