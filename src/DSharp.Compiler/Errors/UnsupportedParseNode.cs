namespace DSharp.Compiler.Errors
{
    internal class UnsupportedParseNode : BaseError
    {
        public UnsupportedParseNode(string message, string location) 
            : base(message, location)
        {
        }
    }
}
