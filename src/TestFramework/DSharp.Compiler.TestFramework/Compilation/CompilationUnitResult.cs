using System;
using System.Collections.Generic;
using System.Linq;
using DSharp.Compiler.Errors;

namespace DSharp.Compiler.TestFramework.Compilation
{
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
}
