using System;

[assembly:ScriptAssembly("SimpleTest")] //Remove once we can get it from the AssemblyName parameter

namespace DSharpSdkTestLib
{
    public class Simple
    {
        private readonly string something;

        public Simple(string something)
        {
            this.something = something;
        }
    }
}
