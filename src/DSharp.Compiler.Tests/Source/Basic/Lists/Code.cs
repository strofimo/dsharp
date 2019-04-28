using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Testing;
using System.Runtime.CompilerServices;

[assembly: ScriptAssembly("test")]

namespace ListTests
{

    public class PublicClass
    {

        public PublicClass()
        {
            List<string> list = new List<string>();
            list.Add("one");
            list.AddRange("two", "three");

            string[] array = list.ToArray();

            int[] integersSource = { 1, 2, 3 };
            List<int> listOfIntegers = new List<int>(integersSource);
            listOfIntegers.Add(4);

            List<int> other = new List<int>(5);
        }
    }
}
