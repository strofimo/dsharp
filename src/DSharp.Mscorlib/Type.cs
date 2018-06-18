using System.Collections;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace System
{
    /// <summary>
    /// The Type data type which is mapped to the Function type in Javascript.
    /// </summary>
    [ScriptIgnoreNamespace]
    [ScriptImport]
    public sealed class Type
    {
        [ScriptName("$base")]
        [ScriptField]
        public extern Type BaseType { get; }

        public extern string Name { get; }

        //TODO: Look at moving out of this class
        [ScriptField]
        public extern Dictionary Prototype { get; }

        [ScriptAlias("ss.type")]
        public extern static Type GetType(string typeName);

        [ScriptAlias("ss.canAssign")]
        public extern bool IsAssignableFrom(Type type);

        [ScriptAlias("ss.isClass")]
        public extern static bool IsClass(Type type);

        [ScriptAlias("ss.isInterface")]
        public extern static bool IsInterface(Type type);

        [ScriptAlias("ss.instanceOf")]
        public extern bool IsInstanceOfType(object instance);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public extern static Type GetTypeFromHandle(RuntimeTypeHandle typeHandle);
    }
}
