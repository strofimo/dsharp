using System;
using System.Collections.Generic;

namespace ScriptSharp
{
    public sealed class CompilerOptions
    {
        public ICollection<string> Defines { get; set; }

        public IStreamSource DocCommentFile { get; set; }

        public IStreamSourceResolver IncludeResolver { get; set; }

        public bool Minimize { get; set; }

        public ICollection<string> References { get; set; }

        public ICollection<IStreamSource> Resources { get; set; }

        public IStreamSource ScriptFile { get; set; }

        public ICollection<IStreamSource> Sources { get; set; }

        public ScriptInfo ScriptInfo { get; }

        public bool EnableDocComments
        {
            get { return DocCommentFile != null; }
        }

        public CompilerOptions()
        {
            ScriptInfo = new ScriptInfo();
        }

        public bool Validate(out string errorMessage)
        {
            errorMessage = String.Empty;

            if (References.Count == 0)
            {
                errorMessage = "You must specify a list of valid assembly references.";
                return false;
            }

            if (Sources.Count == 0)
            {
                errorMessage = "You must specify a list of valid source files.";
                return false;
            }

            if (ScriptFile == null)
            {
                errorMessage = "You must specify a valid output script file.";
                return false;
            }

            return true;
        }
    }
}
