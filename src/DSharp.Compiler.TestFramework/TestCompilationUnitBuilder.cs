using System.Collections.Generic;
using ScriptSharp;

namespace DSharp.Compiler.TestFramework
{
    public class TestCompilationUnitBuilder : ICompilationUnitBuilder
    {
        public CompilerOptions Options { get; } = new CompilerOptions
        {
            Defines = new HashSet<string>(),
            Sources = new HashSet<IStreamSource>(),
            Resources = new HashSet<IStreamSource>(),
            References = new HashSet<string>()
        };

        public ICompilationUnit Build()
        {
            //Options.InternalTestMode = true;
            return new TestCompilationUnit(Options);
        }
    }
}
