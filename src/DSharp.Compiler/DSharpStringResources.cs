namespace DSharp.Compiler
{
    public static class DSharpStringResources
    {
        public const string DSHARP_SCRIPT_NAME = "ds";

        public const string SCRIPT_ALIAS_ATTRIBUTE = "ScriptAlias";
        public const string DSHARP_MEMBER_NAME_ATTRIBUTE = "DSharpScriptMemberName";

        public static string ScriptExportMember(string methodName)
        {
            return $"{DSHARP_SCRIPT_NAME}.{methodName}";
        }
    }
}
