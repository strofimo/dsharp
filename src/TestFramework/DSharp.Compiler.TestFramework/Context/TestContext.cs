using System.IO;
using ScriptSharp;

namespace DSharp.Compiler.TestFramework
{
    public class TestContext : ITestContext
    {
        public FileInfo[] SourceFiles { get; internal set; }

        public FileInfo ExpectedOutput { get; internal set; }

        public FileInfo[] References { get; internal set; }

        public string[] Defines { get; internal set; }

        public FileInfo CommentFile { get; internal set; }

        public IStreamSourceResolver StreamSourceResolver { get; internal set; }

        public FileInfo[] Resources { get; internal set; }

        public TestContextCompliationOptions CompilationOptions { get; internal set; }
    }
}