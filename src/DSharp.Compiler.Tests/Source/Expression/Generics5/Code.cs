using System;
using System.Collections.Generic;

[assembly: ScriptAssembly("test")]

namespace TypeTests
{
    public class Program
    {
        public void Main()
        {
            ClassWithGenericOverload<int> subject = new ClassWithGenericOverload<int>();
            subject.Method();
            subject.Method<string>();
        }
    }

    public class ClassWithGenericOverload<T1>
    {
        public extern void Method();

        public void Method<T2>()
        {

        }
    }
}
