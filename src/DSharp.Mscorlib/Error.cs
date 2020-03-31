using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace System
{
    /// <summary>
    /// Equivalent to the Error type in Javascript.
    /// </summary>
    [ScriptIgnoreNamespace]
    [ScriptImport]
    [ScriptName("Error")]
    public class Error
    {
        public Error(string message) { }

        [ScriptField]
        public extern Error InnerException { get; }

        [ScriptField]
        public extern string Message { get; }

        [ScriptField]
        [ScriptName("stack")]
        public extern string StackTrace { get; }

        [ScriptField]
        public extern object this[string key] { get; }

        [DSharpScriptMemberName("error")] //TODO: Should be createError
        public extern static Error Create(string message, Dictionary<string, object> errorInfo);

        [DSharpScriptMemberName("error")] //TODO: Should be createError
        public extern static Error Create(string message, Dictionary<string, object> errorInfo, Error innerException);
    }
}
