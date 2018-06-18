using System.Runtime.CompilerServices;

namespace System.Collections
{
    [ScriptIgnoreNamespace]
    [ScriptImport]
    [ScriptName("Object")]
    public sealed class Dictionary : IEnumerable
    {
        public Dictionary() { }

        public Dictionary(params object[] nameValuePairs) { }

        public extern int Count { get; }

        public extern string[] Keys { get; }

        [ScriptField]
        public extern object this[string key] { get; set; }

        [ScriptAlias("ss.clearKeys")]
        public extern void Clear();

        [ScriptAlias("ss.keyExists")]
        public extern bool ContainsKey(string key);

        public extern static Dictionary GetDictionary(object o);

        public extern void Remove(string key);

        extern IEnumerator IEnumerable.GetEnumerator();
    }
}
