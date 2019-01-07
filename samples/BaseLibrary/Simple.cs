using System;

[assembly:ScriptAssembly("SimpleTest")] //Remove once we can get it from the AssemblyName parameter

namespace DSharpSdkTestLib
{
    public class Simple
    {
        private readonly string message;

        public Simple(string message)
        {
            this.message = message;
        }

        public string Message
        {
            get
            {
                return message;
            }
        }
    }
}
