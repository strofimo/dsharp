using System;
using System.Runtime.CompilerServices;

[assembly: ScriptAssembly("lib1")]

namespace Library1 {

    [ScriptImport]
    public class ScriptImportedGenericClass<T>
    {

        public ScriptImportedGenericClass()
        {
        }
    }

    public class ReferencedGenericClass<T>
    {

        public ReferencedGenericClass()
        {
        }

        public extern void Method();

        public void Method<T2>(){}
    }
}
