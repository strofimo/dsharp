using System.IO;

namespace DSharp.Compiler.TestFramework
{
    public class TestContext : ITestContext
    {
        public FileInfo SourceCode { get; set; }

        public FileInfo ExpectedOutput { get; set; }
    }
}