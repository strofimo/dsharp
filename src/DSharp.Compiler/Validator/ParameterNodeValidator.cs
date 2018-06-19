using DSharp.Compiler.CodeModel;
using DSharp.Compiler.CodeModel.Members;
using DSharp.Compiler.Errors;

namespace DSharp.Compiler.Validator
{
    internal sealed class ParameterNodeValidator : IParseNodeValidator
    {
        bool IParseNodeValidator.Validate(ParseNode node, CompilerOptions options, IErrorHandler errorHandler)
        {
            ParameterNode paramNode = (ParameterNode)node;

            if (paramNode.Flags == ParameterFlags.Ref || paramNode.Flags == ParameterFlags.Out)
            {
                errorHandler.ReportError(new NodeValidationError("Out and Ref style of parameters are not yet implemented.", paramNode));

                return false;
            }

            return true;
        }
    }
}
