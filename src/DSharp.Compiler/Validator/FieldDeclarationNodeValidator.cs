using DSharp.Compiler.CodeModel;
using DSharp.Compiler.CodeModel.Members;
using DSharp.Compiler.Errors;

namespace DSharp.Compiler.Validator
{
    internal sealed class FieldDeclarationNodeValidator : IParseNodeValidator
    {
        bool IParseNodeValidator.Validate(ParseNode node, CompilerOptions options, IErrorHandler errorHandler)
        {
            FieldDeclarationNode fieldNode = (FieldDeclarationNode)node;

            if (fieldNode.Initializers.Count > 1)
            {
                errorHandler.ReportError(new NodeValidationError("Field declarations are limited to a single field per declaration.", fieldNode));

                return false;
            }

            return true;
        }
    }
}
