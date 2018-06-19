using DSharp.Compiler.CodeModel;
using DSharp.Compiler.CodeModel.Members;
using DSharp.Compiler.Errors;

namespace DSharp.Compiler.Validator
{
    internal sealed class EnumerationFieldNodeValidator : IParseNodeValidator
    {
        bool IParseNodeValidator.Validate(ParseNode node, CompilerOptions options, IErrorHandler errorHandler)
        {
            EnumerationFieldNode enumFieldNode = (EnumerationFieldNode) node;

            object fieldValue = enumFieldNode.Value;

            if (fieldValue == null)
            {
                errorHandler.ReportError(new NodeValidationError("Enumeration fields must have an explicit constant value specified.",enumFieldNode));

                return false;
            }

            if (fieldValue is long || fieldValue is ulong)
            {
                errorHandler.ReportError(new NodeValidationError("Enumeration fields cannot have long or ulong underlying type.",enumFieldNode));
            }

            return true;
        }
    }
}
