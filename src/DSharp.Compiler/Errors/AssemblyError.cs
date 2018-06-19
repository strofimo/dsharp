namespace DSharp.Compiler.Errors
{
    public class AssemblyError : BaseError
    {
        public AssemblyError(string assemblyName, string message)
            : base(assemblyName ?? string.Empty, message)
        {
            AssemblyName = assemblyName;
        }

        public string AssemblyName { get; }
    }
}
