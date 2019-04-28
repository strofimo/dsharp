using System.Runtime.CompilerServices;

namespace System.Collections.Generic
{
    [ScriptImport]
    [ScriptName("IEnumerator")]
    public interface IEnumerator<T>
    {
        [ScriptField]
        T Current { get; }

        bool MoveNext();

        void Reset();
    }
}
