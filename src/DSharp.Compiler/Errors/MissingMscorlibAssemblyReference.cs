namespace DSharp.Compiler.Errors
{
    public class MissingMscorlibAssemblyReference : BaseError
    {
        public MissingMscorlibAssemblyReference() 
            : base($"Missing required mscorlib - {DSharpStringResources.DSHARP_MSCORLIB_ASSEMBLY_NAME}", string.Empty)
        {
        }
    }
}
