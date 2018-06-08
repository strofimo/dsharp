// AttributeNode.cs
// Script#/Core/Compiler
// This source code is subject to terms and conditions of the Apache License, Version 2.0.
//

using System;
using System.Linq;
using DSharp.Compiler.CodeModel.Expressions;
using DSharp.Compiler.CodeModel.Names;

namespace DSharp.Compiler.CodeModel.Attributes
{
    internal sealed class AttributeNode : ParseNode
    {
        private readonly ParseNode arguments;

        private readonly NameNode typeName;

        public AttributeNode(NameNode typeName, ParseNode arguments)
            : base(ParseNodeType.Attribute, typeName.Token)
        {
            this.typeName = (NameNode) GetParentedNode(typeName);
            this.arguments = GetParentedNode(arguments);
        }

        public ParseNodeList Arguments
        {
            get { return ((ExpressionListNode) arguments)?.Expressions; }
        }

        public string TypeName => typeName.Name;

        public static AttributeNode FindAttribute(ParseNodeList attributeNodes, string attributeName)
        {
            if (attributeNodes == null || attributeNodes.Count == 0)
            {
                return null;
            }

            foreach (ParseNode parseNode in attributeNodes)
            {
                AttributeNode attrNode = parseNode as AttributeNode;

                if (attrNode?.TypeName.Equals(attributeName, StringComparison.Ordinal) ?? false)
                {
                    return attrNode;
                }
            }

            return null;
        }

        public static ParseNodeList GetAttributeList(ParseNodeList attributeBlocks)
        {
            if (attributeBlocks == null || attributeBlocks.Count == 0)
            {
                return attributeBlocks;
            }

            ParseNodeList attributes = new ParseNodeList();

            foreach (AttributeBlockNode attributeBlock in attributeBlocks.Cast<AttributeBlockNode>())
            {
                ParseNodeList localAttributes = attributeBlock.Attributes;

                if (localAttributes.Count != 0)
                {
                    attributes.Append(localAttributes);
                }
            }

            return attributes;
        }
    }
}