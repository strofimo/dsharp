using System.Runtime.CompilerServices;

namespace System
{

    [ScriptIgnoreNamespace]
    [ScriptImport]
    [ScriptName("ss")]
    public static class Activator
    {
        public extern static object CreateInstance(Type type);

        public extern static T CreateInstance<T>();

        public extern static object CreateInstance(Type type, params object[] args);
    }
}
