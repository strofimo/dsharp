namespace DSharp.Compiler
{
    public static class DSharpStringResources
    {
        public static readonly string DSHARP_MSCORLIB_ASSEMBLY_NAME = "DSharp.Mscorlib";
        public static readonly string DSHARP_SCRIPT_NAME = "ds";

        public static readonly string SCRIPT_ALIAS_ATTRIBUTE = "ScriptAlias";
        public static readonly string DSHARP_MEMBER_NAME_ATTRIBUTE = "DSharpScriptMemberName";

        public static string ScriptExportMember(string methodName)
        {
            return $"{DSHARP_SCRIPT_NAME}.{methodName}";
        }
    }
}
