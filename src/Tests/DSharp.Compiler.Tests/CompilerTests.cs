using System;
using System.Collections.Generic;
using System.Linq;
using DSharp.Compiler.TestFramework;
using DSharp.Compiler.TestFramework.Context;
using DSharp.Compiler.Tests.Fixtures;
using Xunit;

namespace DSharp.Compiler.Tests
{
    public class CompilerTests : IClassFixture<TestContextFixture>
    {
        private readonly TestContextFixture compilerCompliationFixture;

        private ICompilationUnitFactory CompilationUnitFactory => compilerCompliationFixture?.CompilationUnitFactory;

        private ITestContextFactory TestContextFactory => compilerCompliationFixture?.TestContextFactory;

        public CompilerTests(TestContextFixture compilerCompliationFixture)
        {
            this.compilerCompliationFixture = compilerCompliationFixture;
        }

        [Theory]
        [MemberData(nameof(CompilerOutputTestData))]
        public void TestCompilerOutput(string category, string testName)
        {
            ITestContext testContext = TestContextFactory.GetContext(category, testName);
            ICompilationUnit compilationUnit = CompilationUnitFactory.CreateCompilationUnitBuilder()
                .WithTestContext(testContext)
                .Build();

            Assert.True(compilationUnit.Compile(out ICompilationUnitResult result), result?.WriteErrors());

            string expectedOutput = GetExpectedOutput(testContext);
            Assert.Equal(result.Output, expectedOutput, StringComparer.InvariantCultureIgnoreCase);
        }

        private string GetExpectedOutput(ITestContext testContext)
        {
            return testContext?.ExpectedOutput.OpenText()?.ReadToEnd();
        }

        public static IEnumerable<object[]> CompilerOutputTestData
        {
            get
            {
                yield return new object[] { "Basic", "Lists" };
                yield return new object[] { "Basic", "Guid" };
                yield return new object[] { "Basic", "Conditionals_Debug" };
                yield return new object[] { "Basic", "Conditionals_Trace" };
                yield return new object[] { "Basic", "Dependencies_Baseline_1" };
                yield return new object[] { "Basic", "Dependencies_Baseline_2" };
                yield return new object[] { "Basic", "Dependencies_Baseline_3" };
                yield return new object[] { "Basic", "Dependencies_Baseline_4" };
                yield return new object[] { "Basic", "DocComments" };
                yield return new object[] { "Basic", "Flags" };
                yield return new object[] { "Basic", "Single_Includes" };
                yield return new object[] { "Basic", "Multiple_Includes" };
                yield return new object[] { "Basic", "Default_Loader" };
                yield return new object[] { "Basic", "Simple_Loader" };
                yield return new object[] { "Basic", "AMD_Loader" };
                yield return new object[] { "Basic", "Resources" };
            }
        }
    }
}