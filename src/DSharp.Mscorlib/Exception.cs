using System.Runtime.CompilerServices;

namespace System
{
    [ScriptImport]
    public class Exception : Error
    {
        [ScriptName("name")]
        public string Name;

        [ScriptName("message")]
        public new string Message;

        public extern Exception();

        public extern Exception(string userMessage);
    }
}
