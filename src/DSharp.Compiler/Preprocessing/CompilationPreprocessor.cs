using DSharp.Compiler.Preprocessing.Lowering;
using Microsoft.CodeAnalysis.CSharp;

namespace DSharp.Compiler.Preprocessing
{
    public class CompilationPreprocessor
    {
        private readonly IntermediarySourceManager intermediarySourceManager;

        public CompilationPreprocessor(IntermediarySourceManager intermediarySourceManager = null)
        {
            this.intermediarySourceManager = intermediarySourceManager;
        }

        public CSharpCompilation Preprocess(CSharpCompilation compilation, params ILowerer[] lowerers)
        {
            for (int i = 0; i < compilation.SyntaxTrees.Length; ++i)
            {
                foreach (var lowerer in lowerers)
                {
                    var syntaxTree = compilation.SyntaxTrees[i];
                    var newRoot = lowerer.Apply(compilation, syntaxTree.GetCompilationUnitRoot());
                    compilation = compilation.ReplaceSyntaxTree(syntaxTree, syntaxTree.WithRootAndOptions(newRoot, syntaxTree.Options));
                }
            }

            intermediarySourceManager?.Write(compilation);

            return compilation;
        }
    }
}
