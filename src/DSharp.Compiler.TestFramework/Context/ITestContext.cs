using System.IO;
using ScriptSharp;

namespace DSharp.Compiler.TestFramework
{
    public interface ITestContext
    {
        FileInfo[] SourceFiles { get; }

        FileInfo[] References { get; }

        FileInfo[] Resources { get; }

        string[] Defines { get; }

        FileInfo ExpectedOutput { get; }

        FileInfo CommentFile { get; }

        IStreamSourceResolver StreamSourceResolver { get; }
    }
}