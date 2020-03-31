using System.Runtime.CompilerServices;

namespace System
{
    [ScriptImport]
    public class NotImplementedException : Exception
    {
        public extern NotImplementedException();
    }
}
