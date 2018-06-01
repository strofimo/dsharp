using System.Collections.Generic;
using ScriptSharp;

namespace DSharp.Compiler.TestFramework
{
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

            if (defines != null)
            {
                defines.Add(define);
            }

            return compilationUnitBuilder;
        }
    }
}
