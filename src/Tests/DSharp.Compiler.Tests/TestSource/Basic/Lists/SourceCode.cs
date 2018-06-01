using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

#if SCRIPTSHARP
[assembly: ScriptAssembly("sample")]
#endif

namespace DSharp.Compiler.Tests.TestSource.Basic.Lists
{
    public class PulicClass
    {
        public PulicClass()
        {
            List<string> list = new List<string>();

            list.Add("one");
            list.AddRange(new string[] { "two", "three" });

            string[] array = list.ToArray();
        }
    }
}