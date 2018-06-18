
using System.Runtime.CompilerServices;

namespace System.Collections
{
    [ScriptIgnoreNamespace]
    [ScriptImport]
    public sealed class DictionaryEntry
    {
        internal DictionaryEntry() { }

        [ScriptField]
        public extern string Key { get; }

        [ScriptField]
        public extern object Value { get; }
    }
}
