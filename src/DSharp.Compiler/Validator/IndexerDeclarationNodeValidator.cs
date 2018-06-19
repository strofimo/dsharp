using DSharp.Compiler.CodeModel;
using DSharp.Compiler.CodeModel.Members;
using DSharp.Compiler.Errors;

namespace DSharp.Compiler.Validator
{
    internal sealed class IndexerDeclarationNodeValidator : IParseNodeValidator
    {
        bool IParseNodeValidator.Validate(ParseNode node, CompilerOptions options, IErrorHandler errorHandler)
        {
            IndexerDeclarationNode indexerNode = (IndexerDeclarationNode)node;

            if ((indexerNode.Modifiers & Modifiers.New) != 0)
            {
                errorHandler.ReportError(new NodeValidationError("The new modifier is not supported.", indexerNode));

                return false;
            }

            if (indexerNode.GetAccessor == null)
            {
                errorHandler.ReportError(new NodeValidationError("Set-only properties are not supported. Use a set method instead.", indexerNode));

                return false;
            }

            return true;
        }
    }
}
