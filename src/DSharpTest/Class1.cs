using System;

[assembly: ScriptAssembly("test")]

namespace DSharpTest
{
    public class Class1
    {
        private readonly string values;

        public Class1()
        {
            values = "adasd";
        }
    }

    internal class InternalClass
    {
        private readonly string api;

        public InternalClass(string api)
        {
            this.api = api;
        }
    }
}
