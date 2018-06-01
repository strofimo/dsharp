using System.IO;

namespace DSharp.Compiler.TestFramework
{
    public interface ITestContext
    {
        FileInfo SourceCode { get; }

        FileInfo ExpectedOutput { get; }
    }
}