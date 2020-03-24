using System.Collections.Generic;
using System.IO;
using Microsoft.CodeAnalysis.CSharp;

namespace DSharp.Compiler.Preprocessing
{
    public class IntermediarySourceManager
    {
        private readonly HashSet<string> paths;
        private readonly string intermediarySourceFolder;

        public IntermediarySourceManager(string intermediarySourceFolder)
        {
            this.paths = new HashSet<string>();
            this.intermediarySourceFolder = intermediarySourceFolder;
        }

        public void Write(CSharpCompilation compilation)
        {
            if(string.IsNullOrEmpty(intermediarySourceFolder))
            {
                return;
            }

            Directory.CreateDirectory(intermediarySourceFolder);

            foreach (var syntaxTree in compilation.SyntaxTrees)
            {
                var filePath = syntaxTree.FilePath;
                var extension = $".g{Path.GetExtension(filePath)}";
                var fileName = Path.GetFileNameWithoutExtension(filePath);
                fileName = GetAvailableFileName(fileName, extension);
                var path = Path.Combine(intermediarySourceFolder, fileName);
                File.WriteAllText(path, syntaxTree.GetText().ToString());
            }
        }

        private string GetAvailableFileName(string fileName, string extension, int increment = 0)
        {
            var updatedName = increment > 0
                ? fileName + $"_{increment}{extension}"
                : fileName + extension;

            if (paths.Add(updatedName))
            {
                return updatedName;
            }

            return GetAvailableFileName(fileName, extension, increment + 1);
        }
    }
}
