namespace DSharp.Compiler.CodeModel.Names
{
    /// <summary>
    /// Represents a name node for an unresolved var where the required metadata isn't available
    /// </summary>
    internal class UnresolvedVarNameNode : NameNode
    {
        public UnresolvedVarNameNode(NameNode nameNode)
            : base(ParseNodeType.Name, nameNode.Token)
        {
            LookupNameNode = nameNode;
        }

        public NameNode LookupNameNode { get; }

        protected sealed override ParseNodeList List => new ParseNodeList(this);
    }
}
