namespace DSharp.Compiler.TestFramework
{
    public class TestCompilationUnitFactory : ICompilationUnitFactory
    {
        private const string MSCORLIB = "mscorlib.dll";

        public ICompilationUnitBuilder CreateCompilationUnitBuilder()
        {
            return new TestCompilationUnitBuilder()
                .AddReferences(MSCORLIB)
                .AddDefines("SCRIPTSHARP");
        }
    }
}
