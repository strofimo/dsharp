using System.IO;
using DSharp.Compiler.TestFramework;
using DSharp.Compiler.TestFramework.Data;

namespace DSharp.Compiler.Tests.Fixtures
{
    public class TestContextFixture
    {
        private const string SOURCE_DIRECTORY = "Source";
        private const string COMPILER_TESTS_FILE_NAME = "CompilerTests.json";

        public ICompilationUnitFactory CompilationUnitFactory { get; }

        public ITestContextFactory TestContextFactory { get; }

        public ITestDataProvider DataProvider { get; }

        public string SourcePath { get; }

        public TestContextFixture()
        {
            SourcePath = Path.Combine(Directory.GetCurrentDirectory(), SOURCE_DIRECTORY);

            CompilationUnitFactory = new TestCompilationUnitFactory();
            DataProvider = new TestDataProvider(Path.Combine(SourcePath, COMPILER_TESTS_FILE_NAME));
            TestContextFactory = new TestContextFactory(SourcePath, DataProvider);
        }
    }
}