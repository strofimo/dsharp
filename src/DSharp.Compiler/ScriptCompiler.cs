using System;
using System.Diagnostics;
using System.Linq;
using DSharp.Compiler.Errors;

namespace DSharp.Compiler
{
    public sealed class ScriptCompiler : IErrorHandler
    {
        private readonly IErrorHandler errorHandler;

        public ScriptCompiler()
            : this(null)
        {
        }

        public ScriptCompiler(IErrorHandler errorHandler)
        {
            this.errorHandler = errorHandler;
        }

        public bool HasErrored { get; set; }

        public bool Compile(CompilerOptions options)
        {
            if (options.DebugMode)
            {
                Debugger.Launch();
            }

            ICompilerStrategy compilerStrategy = null;
            if (options.Features.Contains("Roslyn"))
            {
                compilerStrategy = new RoslynCompilerStrategy();
            }
            else
            {
                compilerStrategy = new LegacyCompilerStrategy();
            }

            compilerStrategy.Compile(options, errorHandler);
            return HasErrored;
        }
        void IErrorHandler.ReportError(CompilerError error)
        {
            HasErrored = true;
            if (errorHandler != null)
            {
                errorHandler.ReportError(error);
                return;
            }

            LogError(error);
        }

        private void LogError(CompilerError error)
        {
            if (error.ColumnNumber != null || error.LineNumber != null)
            {
                Console.Error.WriteLine($"{error.File}({error.LineNumber.GetValueOrDefault()},{error.ColumnNumber.GetValueOrDefault()})");
            }

            Console.Error.WriteLine(error.Description);
        }
    }
}
