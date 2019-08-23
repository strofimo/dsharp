using System.Runtime.CompilerServices;

namespace System.Collections.Generic
{
    [ScriptImport]
    [ScriptName("Queue")]
    public sealed class Queue<T> : IEnumerable<T>
    {
        [ScriptField]
        public extern int Count { get; }

        public extern void Clear();

        public extern bool Contains(T item);

        public extern T Dequeue();

        public extern void Enqueue(T item);

        public extern T Peek();

        //
        // Summary:
        //     Copies the System.Collections.Generic.Queue`1 elements to a new array.
        //
        // Returns:
        //     A new array containing elements copied from the System.Collections.Generic.Queue`1.
        public extern T[] ToArray();

        [DSharpScriptMemberName("enumerate")]
        public extern IEnumerator<T> GetEnumerator();

        [DSharpScriptMemberName("enumerate")]
        extern IEnumerator IEnumerable.GetEnumerator();
    }
}
