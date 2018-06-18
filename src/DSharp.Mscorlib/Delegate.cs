using System.Runtime.CompilerServices;

namespace System
{
    [ScriptImport]
    public abstract class Delegate
    {
        protected Delegate(object target, string method) { }

        protected Delegate(Type target, string method) { }

        [ScriptAlias("ss.bindAdd")]
        public extern static Delegate Combine(Delegate a, Delegate b);

        [ScriptAlias("ss.bind")]
        public extern static Delegate Create(Function f, object instance);

        [ScriptAlias("ss.bindSub")]
        public extern static Delegate Remove(Delegate source, Delegate value);
    }
}
