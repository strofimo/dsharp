using System;
using System.Collections.Generic;
using System.Linq;
using DSharp.Compiler.Errors;
using DSharp.Compiler.TestFramework.Compilation;
using DSharp.Compiler.TestFramework.Context;
using DSharp.Compiler.Tests.Fixtures;
using Xunit;

namespace DSharp.Compiler.Tests
{
    public class CompilerValidationTests : IClassFixture<TestContextFixture>
    {
        private static readonly IEqualityComparer<IError> compilerErrorTypeComparer;
        private static readonly IDictionary<string, Action<IEnumerable<IError>>> compilerErrorTestFunctions;

        private readonly TestContextFixture compilerCompliationFixture;

        private ICompilationUnitFactory CompilationUnitFactory => compilerCompliationFixture?.CompilationUnitFactory;

        private ITestContextFactory TestContextFactory => compilerCompliationFixture?.TestContextFactory;

        static CompilerValidationTests()
        {
            compilerErrorTypeComparer = new CompilerErrorTypeEqualityComparer();
            compilerErrorTestFunctions = new Dictionary<string, Action<IEnumerable<IError>>>()
            {
                ["ConflictingTypes"] = CreateContainsErrorsFunction(compilerErrorTypeComparer, new GeneralError(string.Empty)),
                ["Exceptions"] = CreateContainsErrorMessagesFunction(
                    DSharpStringResources.DSHARP_NODE_VALIDATION_ERROR_TRY_CATCH,
                    DSharpStringResources.DSHARP_THROW_NODE_VALIDATION_ERROR),
                ["ImplicitEnums"] = CreateContainsErrorMessagesFunction(
                    DSharpStringResources.DSHARP_ENUM_CONSTANT_VALUE_MISSING_ERROR,
                    DSharpStringResources.DSHARP_ENUM_VALUE_TYPE_ERROR),
                ["InlineScript"] = CreateContainsErrorMessagesFunction(
                    DSharpStringResources.DSHARP_SCRIPT_LITERAL_CONSTANT_ERROR,
                    DSharpStringResources.DSHARP_SCRIPT_LITERAL_FORMAT_ERROR),
                ["Keywords"] = CreateContainsErrorMessagesFunction(
                    string.Format(DSharpStringResources.DSHARP_RESERVED_KEYWORD_ERROR_FORMAT, DSharpStringResources.DSHARP_SCRIPT_NAME),
                    string.Format(DSharpStringResources.DSHARP_RESERVED_KEYWORD_ERROR_FORMAT, "instanceof")
                    ),
                ["Modules"] = CreateContainsErrorMessagesFunction(
                    DSharpStringResources.DSHARP_SCRIPT_MODULE_NON_INTERNAL_CLASS_ERROR,
                    DSharpStringResources.DSHARP_SCRIPT_MODULE_NON_STATIC_CONSTRUCTOR
                    ),
                ["NestedTypes"] = CreateContainsErrorMessagesFunction(
                    DSharpStringResources.DSHARP_NESTED_TYPE_ERROR,
                    DSharpStringResources.DSHARP_NESTED_TYPE_ERROR
                    ),
                ["Overloads"] = CreateContainsErrorMessagesFunction(
                    DSharpStringResources.DSHARP_EXTERN_IMPLEMENTATION_FOUND_ERROR,
                    DSharpStringResources.DSHARP_EXTERN_STATIC_MEMBER_MISMATCH_ERROR,
                    DSharpStringResources.DSHARP_EXTERN_STATIC_MEMBER_MISMATCH_ERROR
                    ),
                ["Records"] = CreateContainsErrorMessagesFunction(
                    DSharpStringResources.DSHARP_SCRIPT_OBJECT_ATTRIBUTE_ERROR,
                    DSharpStringResources.DSHARP_SCRIPT_OBJECT_CLASS_INHERITENCE_ERROR,
                    DSharpStringResources.DSHARP_SCRIPT_OBJECT_MEMBER_VIOLATION_ERROR,
                    DSharpStringResources.DSHARP_SCRIPT_OBJECT_MEMBER_VIOLATION_ERROR
                    ),
                ["ScriptExtension"] = CreateContainsErrorMessagesFunction(
                    DSharpStringResources.DSHARP_EXTENSION_ATTRIBUTE_ERROR,
                    DSharpStringResources.DSHARP_SCRIPT_EXTENSION_MEMBER_VIOLATION_ERROR
                    ),
            };
        }

        public CompilerValidationTests(TestContextFixture compilerCompliationFixture)
        {
            this.compilerCompliationFixture = compilerCompliationFixture;
        }

        [Theory(DisplayName = "Compiler Error Validation")]
        [MemberData(nameof(TestCompilerErrorsData))]
        public void TestCompilerErrors(string testName)
        {
            IList<string> sourceFiles = TestContextFactory.GetTestSourceFiles("Validation", testName);
            ICompilationUnit compilationUnit = CompilationUnitFactory.CreateCompilationUnitBuilder()
                .AddSourceFiles(sourceFiles.ToArray())
                .Build();

            bool compilationSuccess = compilationUnit.Compile(out ICompilationUnitResult compilationUnitResult);
            Assert.False(compilationSuccess, "Expected compilation to fail");

            compilerErrorTestFunctions.TryGetValue(testName, out Action<IEnumerable<IError>> compilerErrorResultValidator);

            compilerErrorResultValidator.Invoke(compilationUnitResult.Errors);
        }
        public static IEnumerable<object[]> TestCompilerErrorsData
        {
            get { return compilerErrorTestFunctions.Keys.Select(key => new object[] { key }); }
        }

        private static Action<IEnumerable<IError>> CreateContainsErrorsFunction(IEqualityComparer<IError> equalityComparer = null, params IError[] expectedErrors)
        {
            return new Action<IEnumerable<IError>>((errors) =>
            {
                Assert.Equal(expectedErrors.Length, errors.Count());

                foreach (IError expectedError in expectedErrors)
                {
                    Assert.Contains(expectedError, errors, equalityComparer ?? EqualityComparer<IError>.Default);
                }
            });
        }

        private static Action<IEnumerable<IError>> CreateContainsErrorMessagesFunction(params string[] expectedErrorMessages)
        {
            return new Action<IEnumerable<IError>>((errors) =>
            {
                IList<string> errorMessages = errors.Select(error => error.Message).ToList();

                Assert.Equal(expectedErrorMessages.Length, errorMessages.Count);

                foreach (string expectedErrorMessage in expectedErrorMessages)
                {
                    Assert.Contains(expectedErrorMessage, errorMessages, StringComparer.InvariantCultureIgnoreCase);
                }
            });
        }
    }
}
