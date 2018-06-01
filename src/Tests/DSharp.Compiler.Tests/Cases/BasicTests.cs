using ScriptSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
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

        [Fact]
        public void TestLists()
        {
            ITestContext testContext = GetScriptTestContext("Basic", "Lists");
            ICompilationUnit compilationUnit = compilationUnitFactory.CreateCompilationUnitBuilder()
                .AddSourceFiles(testContext.SourceCode.FullName)
                .UseDebug()
                .Build();

            Assert.True(compilationUnit.Compile(out ICompilationUnitResult result), result?.WriteErrors());

            string expectedOutput = GetExpectedOutput(testContext);
            Assert.Equal(result.Output, expectedOutput, StringComparer.InvariantCultureIgnoreCase);
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

    public interface ITestContext
    {
        FileInfo SourceCode { get; }

        FileInfo ExpectedOutput { get; }
    }

    internal class TestContext : ITestContext
    {
        public FileInfo SourceCode { get; set; }

        public FileInfo ExpectedOutput { get; set; }
    }

    public interface ICompilationUnit
    {
        bool Compile(out ICompilationUnitResult result);
    }

    public interface ICompilationUnitResult
    {
        string Output { get; }

        IError[] Errors { get; }
    }

    public interface ICompilationUnitBuilder
    {
        CompilerOptions Options { get; }

        ICompilationUnit Build();
    }

    public interface ICompilationUnitFactory
    {
        ICompilationUnitBuilder CreateCompilationUnitBuilder();
    }

    public class TestCompilationUnitFactory : ICompilationUnitFactory
    {
        private const string MSCORLIB = "mscorlib.dll";

        public ICompilationUnitBuilder CreateCompilationUnitBuilder()
        {
            return new TestCompilationUnitBuilder()
                .AddReferences(MSCORLIB)
                .AddDefine("SCRIPTSHARP");
        }
    }

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

    internal class TestCompilationUnit : ICompilationUnit, IErrorHandler
    {
        private readonly ScriptCompiler scriptCompiler;
        private readonly CompilerOptions compilerOptions;
        private readonly List<IError> compilationErrors;

        public TestCompilationUnit(CompilerOptions compilerOptions)
        {
            this.compilerOptions = compilerOptions ?? throw new ArgumentNullException(nameof(compilerOptions));
            this.compilerOptions.ScriptFile = new InMemoryStream();
            scriptCompiler = new ScriptCompiler(this);
            compilationErrors = new List<IError>();
        }

        public bool Compile(out ICompilationUnitResult compilationUnitResult)
        {
            bool compilerSuccess = scriptCompiler.Compile(compilerOptions);

            compilationUnitResult = CreateResult(compilerOptions.ScriptFile as InMemoryStream);
            compilationErrors.Clear();

            return compilerSuccess;
        }

        private ICompilationUnitResult CreateResult(InMemoryStream outputStream)
        {
            if (compilationErrors.Count > 0)
            {
                return CompilationUnitResult.CreateErrorResult(compilationErrors);
            }

            return CompilationUnitResult.CreateResult(outputStream.GeneratedOutput);
        }

        void IErrorHandler.ReportError(string errorMessage, string location)
        {
            compilationErrors.Add(new CompilationError
            {
                Message = errorMessage,
                Location = location
            });
        }
    }

    public sealed class InMemoryStream : IStreamSource
    {
        private readonly string name;

        public string GeneratedOutput { get; private set; }

        string IStreamSource.FullName
        {
            get { return name; }
        }

        string IStreamSource.Name
        {
            get { return name; }
        }

        public InMemoryStream() => name = Guid.NewGuid().ToString();

        void IStreamSource.CloseStream(Stream stream)
        {
            MemoryStream memoryStream = (MemoryStream)stream;
            byte[] buffer = memoryStream.GetBuffer();

            GeneratedOutput = Encoding.UTF8.GetString(buffer, 0, (int)memoryStream.Length);
            memoryStream.Close();
        }

        Stream IStreamSource.GetStream()
        {
            return new MemoryStream();
        }
    }

    public class CompilationUnitResult : ICompilationUnitResult
    {
        public string Output { get; }

        public IError[] Errors { get; }

        private CompilationUnitResult(IError[] errors)
        {
            Errors = errors;
        }

        private CompilationUnitResult(string output)
        {
            Output = output;
            Errors = Array.Empty<IError>();
        }

        public static ICompilationUnitResult CreateErrorResult(IEnumerable<IError> errors)
        {
            return new CompilationUnitResult(errors.ToArray());
        }

        public static ICompilationUnitResult CreateResult(string output)
        {
            return new CompilationUnitResult(output);
        }
    }

    public interface IError
    {
        string Message { get; }

        string Location { get; }
    }

    public class CompilationError : IError
    {
        public string Message { get; set; }

        public string Location { get; set; }
    }

    public static class CompilationUnitBuilderExtensions
    {
        public static ICompilationUnitBuilder AddReferences(this ICompilationUnitBuilder compilationUnitBuilder, params string[] references)
        {
            ICollection<string> compilationReferences = compilationUnitBuilder.Options.References;

            foreach (string reference in references)
            {
                compilationReferences.Add(reference);
            }

            return compilationUnitBuilder;
        }

        public static ICompilationUnitBuilder AddSourceFiles(this ICompilationUnitBuilder compilationUnitBuilder, params string[] sourceFiles)
        {
            ICollection<IStreamSource> sources = compilationUnitBuilder.Options.Sources;

            foreach (string sourcePath in sourceFiles)
            {
                IStreamSource inputSource = new FileInputSource(sourcePath);
                sources.Add(inputSource);
            }

            return compilationUnitBuilder;
        }

        //TODO: Make this better!
        public static ICompilationUnitBuilder UseDebug(this ICompilationUnitBuilder compilationUnitBuilder)
        {
            return compilationUnitBuilder
                .AddDefine("DEBUG");
        }

        //TODO: Make this better!
        public static ICompilationUnitBuilder UseTrace(this ICompilationUnitBuilder compilationUnitBuilder)
        {
            return compilationUnitBuilder
                .AddDefine("TRACE");
        }

        public static ICompilationUnitBuilder AddDefine(this ICompilationUnitBuilder compilationUnitBuilder, string define)
        {
            ISet<string> defines = compilationUnitBuilder.Options.Defines as ISet<string>;

            if(defines != null)
            {
                defines.Add(define);
            }

            return compilationUnitBuilder;
        }
    }

    public static class CompilationUnitResultExtensions
    {
        public static string WriteErrors(this ICompilationUnitResult compilationUnitResult)
        {
            IEnumerable<string> messages = compilationUnitResult.Errors?
                .Select(err => err.Message)
                .Where(message => !string.IsNullOrWhiteSpace(message)) ?? Array.Empty<string>();
            string errorList = string.Join(", ", messages);
            return $"Compilation Errors: {errorList}";
        }
    }

    public class FileInputSource : IStreamSource
    {
        private readonly FileInfo fileInfo;
        private Stream openStream;

        public string FullName
        {
            get { return fileInfo?.FullName; }
        }

        public string Name
        {
            get { return fileInfo?.Name; }
        }

        public FileInputSource(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            fileInfo = new FileInfo(filePath);

            if (!fileInfo.Exists)
            {
                throw new FileNotFoundException("File Stream not found", filePath);
            }
        }

        void IStreamSource.CloseStream(Stream stream)
        {
            if (stream != openStream)
            {
                return;
            }

            openStream.Dispose();
        }

        Stream IStreamSource.GetStream()
        {
            return openStream = fileInfo.OpenRead();
        }
    }
}