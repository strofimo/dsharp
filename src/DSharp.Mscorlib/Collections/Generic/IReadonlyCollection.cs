using System.Runtime.CompilerServices;

namespace System.Collections.Generic
{
    [ScriptImport]
    [ScriptName("ICollection")]
    public interface IReadonlyCollection<T> : IEnumerable<T>
    {
        [ScriptField]
        [ScriptName("length")]
        int Count { get; }

        [ScriptField]
        T this[int index] { get; }
    }
}
