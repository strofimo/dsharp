using DSharp.Compiler.CodeModel;
using DSharp.Compiler.CodeModel.Statements;
using DSharp.Compiler.Errors;

namespace DSharp.Compiler.Validator
{
    internal sealed class TryNodeValidator : IParseNodeValidator
    {
        bool IParseNodeValidator.Validate(ParseNode node, CompilerOptions options, IErrorHandler errorHandler)
        {
            TryNode tryNode = (TryNode)node;

            if (tryNode.CatchClauses != null && tryNode.CatchClauses.Count > 1)
            {
                errorHandler.ReportError(new NodeValidationError("Try/Catch statements are limited to a single catch clause.", tryNode));

                return false;
            }

            return true;
        }
    }
}
