using System;
using System.Diagnostics;
using DSharpSdkTestLib;

[assembly: ScriptAssembly("LibraryWithDependancy")] //Remove once we can get it from the AssemblyName parameter

namespace DependantLibrary
{
    public class SomethingAwesome
    {
        private readonly Simple simple;

        public SomethingAwesome(string message)
        {
            simple = new Simple(message);
        }

        public void Speak()
        {
            Debug.WriteLine(simple.Message);
        }
    }
}
