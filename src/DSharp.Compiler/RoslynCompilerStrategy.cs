using System;
using DSharp.Compiler.Errors;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace DSharp.Compiler
{
    public class RoslynCompilerStrategy : ICompilerStrategy
    {
        public void Compile(CompilerOptions options, IErrorHandler errorHandler)
        {
            var syntaxTrees = ParseSyntaxTrees(options);
        }

        private IEquatable<CSharpSyntaxTree> ParseSyntaxTrees(CompilerOptions options)
        {
            
        }
    }
}
