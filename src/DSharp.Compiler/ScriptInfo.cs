namespace DSharp.Compiler
{
    public sealed class ScriptInfo
    {
        //TODO: Replace with new loader code
        private static readonly string DefaultScriptTemplate = @"
""use strict"";

var {name} = (function($global){
  {script}
  return $exports;
})(self);
";

        public ScriptInfo()
        {
            Template = DefaultScriptTemplate;
        }

        public string Copyright { get; set; }

        public string Description { get; set; }

        public string Template { get; set; }

        public string Version { get; set; }
    }
}
