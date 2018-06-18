using System.Collections;
using System.Runtime.CompilerServices;

namespace System
{
    /// <summary>
    /// Equivalent to the Error type in Javascript.
    /// </summary>
    [ScriptIgnoreNamespace]
    [ScriptImport]
    [ScriptName("Error")]
    public class Exception
    {
        public Exception(string message) { }

        [ScriptField]
        public extern Exception InnerException { get; }

        [ScriptField]
        public extern string Message { get; }

        [ScriptField]
        [ScriptName("stack")]
        public extern string StackTrace { get; }

        [ScriptField]
        public extern object this[string key] { get; }

        [ScriptAlias("ss.error")]
        public extern static Exception Create(string message, Dictionary errorInfo);

        [ScriptAlias("ss.error")]
        public extern static Exception Create(string message, Dictionary errorInfo, Exception innerException);
    }
}
