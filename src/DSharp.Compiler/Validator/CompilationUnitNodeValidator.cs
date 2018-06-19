using DSharp.Compiler.CodeModel;
using DSharp.Compiler.CodeModel.Attributes;
using DSharp.Compiler.CodeModel.Expressions;
using DSharp.Compiler.CodeModel.Types;
using DSharp.Compiler.Errors;

namespace DSharp.Compiler.Validator
{
    internal sealed class CompilationUnitNodeValidator : IParseNodeValidator
    {
        bool IParseNodeValidator.Validate(ParseNode node, CompilerOptions options, IErrorHandler errorHandler)
        {
            CompilationUnitNode compilationUnitNode = (CompilationUnitNode)node;

            foreach (AttributeBlockNode attribBlock in compilationUnitNode.Attributes)
            {
                AttributeNode scriptNamespaceNode =
                    AttributeNode.FindAttribute(attribBlock.Attributes, "ScriptNamespace");

                if (scriptNamespaceNode != null)
                {
                    string scriptNamespace = (string)((LiteralNode)scriptNamespaceNode.Arguments[0]).Value;

                    if (Utility.IsValidScriptNamespace(scriptNamespace) == false)
                    {
                        errorHandler.ReportError(new NodeValidationError("A script namespace must be a valid script identifier.", scriptNamespaceNode));
                    }
                }
            }

            foreach (ParseNode childNode in compilationUnitNode.Members)
                if (!(childNode is NamespaceNode))
                {
                    errorHandler.ReportError(new NodeValidationError("Non-namespaced types are not supported.", childNode));

                    return false;
                }

            return true;
        }
    }
}
