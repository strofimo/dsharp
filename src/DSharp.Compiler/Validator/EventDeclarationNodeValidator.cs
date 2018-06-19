using DSharp.Compiler.CodeModel;
using DSharp.Compiler.CodeModel.Members;
using DSharp.Compiler.Errors;

namespace DSharp.Compiler.Validator
{
    internal sealed class EventDeclarationNodeValidator : IParseNodeValidator
    {
        bool IParseNodeValidator.Validate(ParseNode node, CompilerOptions options, IErrorHandler errorHandler)
        {
            EventDeclarationNode eventNode = (EventDeclarationNode)node;

            if ((eventNode.Modifiers & Modifiers.Static) == 0 &&
                (eventNode.Modifiers & Modifiers.New) != 0)
            {
                errorHandler.ReportError(new NodeValidationError("The new modifier is not supported on instance members.", eventNode));

                return false;
            }

            return true;
        }
    }
}
