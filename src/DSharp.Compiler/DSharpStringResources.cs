namespace DSharp.Compiler
{
    public static class DSharpStringResources
    {
        public static readonly string DSHARP_MSCORLIB_ASSEMBLY_NAME = "DSharp.Mscorlib";
        public static readonly string DSHARP_SCRIPT_NAME = "ds";

        public static readonly string SCRIPT_ALIAS_ATTRIBUTE = "ScriptAlias";
        public static readonly string DSHARP_MEMBER_NAME_ATTRIBUTE = "DSharpScriptMemberName";

        //errors
        public static readonly string NODE_VALIDATION_ERROR_TRY_CATCH = "Try/Catch statements are limited to a single catch clause.";
        public static readonly string THROW_NODE_VALIDATION_ERROR = "Throw statements must specify an exception object.";
        public static readonly string ENUM_CONSTANT_VALUE_MISSING_ERROR = "Enumeration fields must have an explicit constant value specified.";
        public static readonly string ENUM_VALUE_TYPE_ERROR = "Enumeration fields cannot have long or ulong underlying type.";
        public static readonly string SCRIPT_LITERAL_CONSTANT_ERROR = "The argument to Script.Literal must be a constant string.";
        public static readonly string SCRIPT_LITERAL_FORMAT_ERROR = "The argument to Script.Literal must be a valid String.Format string.";
        public static readonly string RESERVED_KEYWORD_ERROR_FORMAT = "{0} is a reserved keyword.";
        public static readonly string SCRIPT_MODULE_NON_INTERNAL_CLASS_ERROR = "ScriptModule attribute can only be set on internal static classes.";
        public static readonly string SCRIPT_MODULE_NON_STATIC_CONSTRUCTOR = "Classes marked with ScriptModule attribute should only have a static constructor.";
        public static readonly string NESTED_TYPE_ERROR = "Only members are allowed inside types. Nested types are not supported.";
        public static readonly string EXTERN_IMPLEMENTATION_FOUND_ERROR = "Extern methods used to declare alternate signatures should have a corresponding non-extern implementation as well.";
        public static readonly string EXTERN_STATIC_MEMBER_MISMATCH_ERROR = "The implemenation method and associated alternate signature methods should have the same access type.";
        public static readonly string SCRIPT_OBJECT_ATTRIBUTE_ERROR = "ScriptObject attribute can only be set on sealed classes.";
        public static readonly string SCRIPT_OBJECT_CLASS_INHERITENCE_ERROR = "Classes marked with ScriptObject must not derive from another class or implement interfaces.";
        public static readonly string SCRIPT_OBJECT_MEMBER_VIOLATION_ERROR = "Classes marked with ScriptObject attribute should only have a constructor and field members.";
        public static readonly string EXTENSION_ATTRIBUTE_ERROR = "ScriptExtension attribute declaration must specify the object being extended.";
        public static readonly string SCRIPT_EXTENSION_MEMBER_VIOLATION_ERROR = "Classes marked with ScriptExtension attribute should only have methods.";

        public static string ScriptExportMember(string methodName)
        {
            return $"{DSHARP_SCRIPT_NAME}.{methodName}";
        }
    }
}
