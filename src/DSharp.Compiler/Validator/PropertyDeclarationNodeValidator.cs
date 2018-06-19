using DSharp.Compiler.CodeModel;
using DSharp.Compiler.CodeModel.Members;
using DSharp.Compiler.Errors;

namespace DSharp.Compiler.Validator
{
    internal sealed class PropertyDeclarationNodeValidator : IParseNodeValidator
    {
        bool IParseNodeValidator.Validate(ParseNode node, CompilerOptions options, IErrorHandler errorHandler)
        {
            PropertyDeclarationNode propertyNode = (PropertyDeclarationNode)node;

            if ((propertyNode.Modifiers & Modifiers.Static) == 0 &&
                (propertyNode.Modifiers & Modifiers.New) != 0)
            {
                errorHandler.ReportError(new NodeValidationError("The new modifier is not supported on instance members.", propertyNode));

                return false;
            }

            return true;
        }
    }
}
