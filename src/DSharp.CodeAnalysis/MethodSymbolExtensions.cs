using System.Linq;
using Microsoft.CodeAnalysis;

namespace DSharp.CodeAnalysis
{
    public static class MethodSymbolExtensions
    {
        private const string ScriptIgnoreGenericArgumentsAttribute = "ScriptIgnoreGenericArguments";

        public static bool HasScriptIgnoreGenericArgumentsAttribute(this IMethodSymbol methodSymbol)
        {
            return methodSymbol
                .GetAttributes()
                .Any(a => a.AttributeClass.Name.StartsWith(ScriptIgnoreGenericArgumentsAttribute));
        }
    }
}
