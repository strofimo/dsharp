﻿namespace System.Runtime.CompilerServices
{
    /// <summary>
    /// Apply to a method to tell the compiler to ignore generic arguments as method parameters
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    [ScriptIgnore]
    public class ScriptIgnoreGenericArgumentsAttribute : Attribute
    {
    }
}
