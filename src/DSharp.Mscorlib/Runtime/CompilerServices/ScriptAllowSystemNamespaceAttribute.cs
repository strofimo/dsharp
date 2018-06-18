namespace System.Runtime.CompilerServices
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    [ScriptIgnore]
    [ScriptImport]
    public sealed class ScriptAllowSystemNamespaceAttribute : Attribute
    {
    }
}
