using DSharp.Compiler.CodeModel;

namespace DSharp.Compiler.Errors
{
    internal class NodeValidationError : IError
    {
        public NodeValidationError(string message, ParseNode parseNode) 
        {
            Message = message;
            ParseNode = parseNode;
        }

        public string Message { get; }

        public ParseNode ParseNode { get; }

        public string Location => ParseNode?.Token?.Location;
    }
}
