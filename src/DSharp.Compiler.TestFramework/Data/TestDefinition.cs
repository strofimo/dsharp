namespace DSharp.Compiler.TestFramework.Data
{
    public class TestDefinition
    {
        public string Name { get; set; }

        public string SourceFolder { get; set; }

        public string[] SourceFiles { get; set; }

        public string[] Resources { get; set; }

        public string ComparisonFile { get; set; }

        public string[] References { get; set; }

        public string[] Defines { get; set; }

        public string DocumentCommentFile { get; set; }
    }
}