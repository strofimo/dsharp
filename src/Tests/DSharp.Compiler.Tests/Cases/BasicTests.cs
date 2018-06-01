using System;
using System.Collections.Generic;
using System.IO;
using DSharp.Compiler.TestFramework;
using Xunit;

namespace DSharp.Compiler.Tests.Cases
{
    public class BasicTests
    {
        private const string ROOT_TEST_DIRECTORY = "TestSource";

        private readonly ICompilationUnitFactory compilationUnitFactory;
        private readonly string rootDirectory;

        public BasicTests()
        {
            compilationUnitFactory = new TestCompilationUnitFactory();
            rootDirectory = Path.Combine(Directory.GetCurrentDirectory(), ROOT_TEST_DIRECTORY);
        }

        [Theory]
        [MemberData(nameof(CompilerOutputTestSource))]
        public void TestCompilerOutput(string category, string testName)
        {
            ITestContext testContext = GetScriptTestContext(category, testName);
            ICompilationUnit compilationUnit = compilationUnitFactory.CreateCompilationUnitBuilder()
                .AddSourceFiles(testContext.SourceCode.FullName)
                .UseDebug()
                .Build();

            Assert.True(compilationUnit.Compile(out ICompilationUnitResult result), result?.WriteErrors());

            string expectedOutput = GetExpectedOutput(testContext);
            Assert.Equal(result.Output, expectedOutput, StringComparer.InvariantCultureIgnoreCase);
        }

        public static IEnumerable<object[]> CompilerOutputTestSource
        {
            get
            {
                yield return new object[] { "Basic", "Lists" };
            }
        }

        private string GetExpectedOutput(ITestContext testContext)
        {
            if (!testContext?.ExpectedOutput?.Exists ?? false)
            {
                throw new FileNotFoundException($"Missing tests expected output file: {testContext?.ExpectedOutput?.FullName}");
            }

            return testContext.ExpectedOutput.OpenText()?.ReadToEnd();
        }

        private ITestContext GetScriptTestContext(string category, string test)
        {
            string testFilesPath = Path.Combine(rootDirectory, category, test);

            return new TestContext
            {
                SourceCode = new FileInfo(Path.Combine(testFilesPath, "SourceCode.cs")),
                ExpectedOutput = new FileInfo(Path.Combine(testFilesPath, "ExpectedOutput.js"))
            };
        }
    }
}