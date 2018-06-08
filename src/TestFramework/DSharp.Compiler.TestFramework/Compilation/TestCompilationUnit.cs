using System;
using System.Collections.Generic;

namespace DSharp.Compiler.TestFramework.Compilation
{
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
}
