namespace DSharp.Compiler.Errors
{
    public class DuplicateAssemblyReferenceError : IError
    {
        public DuplicateAssemblyReferenceError(string assemblyName, string referencePath)
        {
            AssemblyName = assemblyName;
            ReferencePath = referencePath;
        }

        public string AssemblyName { get; }

        public string ReferencePath { get; }

        public string Message
        {
            get
            {
                if (string.Equals(AssemblyName, DSharpStringResources.DSHARP_MSCORLIB_ASSEMBLY_NAME))
                {
                    return $"The core runtime assembly, {DSharpStringResources.DSHARP_MSCORLIB_ASSEMBLY_NAME} must be referenced only once.";
                }

                return $"The referenced assembly '{ReferencePath}' is a duplicate reference.";
            }
        }

        public string Location => ReferencePath;
    }
}
