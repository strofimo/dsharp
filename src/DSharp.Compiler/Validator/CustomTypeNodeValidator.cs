// CustomTypeNodeValidator.cs
// Script#/Core/Compiler
// This source code is subject to terms and conditions of the Apache License, Version 2.0.
//

using System.Collections.Generic;
using DSharp.Compiler.CodeModel;
using DSharp.Compiler.CodeModel.Attributes;
using DSharp.Compiler.CodeModel.Expressions;
using DSharp.Compiler.CodeModel.Members;
using DSharp.Compiler.CodeModel.Names;
using DSharp.Compiler.CodeModel.Tokens;
using DSharp.Compiler.CodeModel.Types;
using DSharp.Compiler.Errors;

namespace DSharp.Compiler.Validator
{
    internal sealed class CustomTypeNodeValidator : IParseNodeValidator
    {
        bool IParseNodeValidator.Validate(ParseNode node, CompilerOptions options, IErrorHandler errorHandler)
        {
            CustomTypeNode typeNode = (CustomTypeNode)node;

            bool extensionRestrictions = false;
            bool moduleRestrictions = false;
            bool recordRestrictions = false;
            bool hasCodeMembers = false;
            ParseNode codeMemberNode = null;

            AttributeNode importedTypeAttribute = AttributeNode.FindAttribute(typeNode.Attributes, "ScriptImport");

            if (importedTypeAttribute != null)
            {
                // This is an imported type definition... we'll assume its valid, since
                // the set of restrictions for such types is fewer, and different, so
                // for now that translates into skipping the members.

                return false;
            }

            if ((typeNode.Modifiers & Modifiers.Partial) != 0 &&
                typeNode.Type != TokenType.Class)
            {
                errorHandler.ReportError(new NodeValidationError("Partial types can only be classes, not enumerations or interfaces.", typeNode));

                return false;
            }

            if (typeNode.Type == TokenType.Class)
            {
                if (typeNode.BaseTypes.Count != 0)
                {
                    NameNode baseTypeNameNode = typeNode.BaseTypes[0] as NameNode;

                    if (baseTypeNameNode != null)
                    {
                        if (string.CompareOrdinal(baseTypeNameNode.Name, "TestClass") == 0)
                        {
                            if ((typeNode.Modifiers & Modifiers.Internal) == 0)
                            {
                                errorHandler.ReportError(new NodeValidationError("Classes derived from TestClass must be marked as internal.", typeNode));
                            }

                            if ((typeNode.Modifiers & Modifiers.Static) != 0)
                            {
                                errorHandler.ReportError(new NodeValidationError("Classes derived from TestClass must not be marked as static.", typeNode));
                            }

                            if ((typeNode.Modifiers & Modifiers.Sealed) == 0)
                            {
                                errorHandler.ReportError(new NodeValidationError("Classes derived from TestClass must be marked as sealed.", typeNode));
                            }

                            if (typeNode.BaseTypes.Count != 1)
                            {
                                errorHandler.ReportError(new NodeValidationError("Classes derived from TestClass cannot implement interfaces.", typeNode));
                            }
                        }
                    }
                }

                AttributeNode objectAttribute = AttributeNode.FindAttribute(typeNode.Attributes, "ScriptObject");

                if (objectAttribute != null)
                {
                    if ((typeNode.Modifiers & Modifiers.Sealed) == 0)
                    {
                        errorHandler.ReportError(new NodeValidationError(DSharpStringResources.SCRIPT_OBJECT_ATTRIBUTE_ERROR, typeNode));
                    }

                    if (typeNode.BaseTypes.Count != 0)
                    {
                        errorHandler.ReportError(new NodeValidationError(DSharpStringResources.SCRIPT_OBJECT_CLASS_INHERITENCE_ERROR, typeNode));
                    }

                    recordRestrictions = true;
                }

                AttributeNode extensionAttribute = AttributeNode.FindAttribute(typeNode.Attributes, "ScriptExtension");

                if (extensionAttribute != null)
                {
                    extensionRestrictions = true;

                    if ((typeNode.Modifiers & Modifiers.Static) == 0)
                    {
                        errorHandler.ReportError(new NodeValidationError("ScriptExtension attribute can only be set on static classes.", typeNode));
                    }

                    if (extensionAttribute.Arguments.Count != 1 ||
                        !(extensionAttribute.Arguments[0] is LiteralNode) ||
                        !(((LiteralNode)extensionAttribute.Arguments[0]).Value is string) ||
                        string.IsNullOrEmpty((string)((LiteralNode)extensionAttribute.Arguments[0]).Value))
                    {
                        errorHandler.ReportError(new NodeValidationError(DSharpStringResources.EXTENSION_ATTRIBUTE_ERROR, typeNode));
                    }
                }

                AttributeNode moduleAttribute = AttributeNode.FindAttribute(typeNode.Attributes, "ScriptModule");

                if (moduleAttribute != null)
                {
                    moduleRestrictions = true;

                    if ((typeNode.Modifiers & Modifiers.Static) == 0 ||
                        (typeNode.Modifiers & Modifiers.Internal) == 0)
                    {
                        errorHandler.ReportError(new NodeValidationError(DSharpStringResources.SCRIPT_MODULE_NON_INTERNAL_CLASS_ERROR, typeNode));
                    }
                }
            }

            if (typeNode.Members != null && typeNode.Members.Count != 0)
            {
                Dictionary<string, object> memberNames = new Dictionary<string, object>();
                bool hasCtor = false;

                foreach (ParseNode genericMemberNode in typeNode.Members)
                {
                    if (!(genericMemberNode is MemberNode))
                    {
                        errorHandler.ReportError(new NodeValidationError(DSharpStringResources.NESTED_TYPE_ERROR, node));

                        continue;
                    }

                    MemberNode memberNode = (MemberNode)genericMemberNode;

                    if ((memberNode.Modifiers & Modifiers.Extern) != 0)
                    {
                        // Extern methods are placeholders for creating overload signatures
                        continue;
                    }

                    if (extensionRestrictions && memberNode.NodeType != ParseNodeType.MethodDeclaration)
                    {
                        errorHandler.ReportError(new NodeValidationError(DSharpStringResources.SCRIPT_EXTENSION_MEMBER_VIOLATION_ERROR, memberNode));
                    }

                    if (moduleRestrictions && memberNode.NodeType != ParseNodeType.ConstructorDeclaration)
                    {
                        errorHandler.ReportError(new NodeValidationError(DSharpStringResources.SCRIPT_MODULE_NON_STATIC_CONSTRUCTOR, memberNode));
                    }

                    if (recordRestrictions &&
                        ((memberNode.Modifiers & Modifiers.Static) != 0 ||
                         memberNode.NodeType != ParseNodeType.ConstructorDeclaration &&
                         memberNode.NodeType != ParseNodeType.FieldDeclaration))
                    {
                        errorHandler.ReportError(new NodeValidationError(DSharpStringResources.SCRIPT_OBJECT_MEMBER_VIOLATION_ERROR, memberNode));
                    }

                    if (memberNode.NodeType == ParseNodeType.ConstructorDeclaration)
                    {
                        if ((memberNode.Modifiers & Modifiers.Static) == 0)
                        {
                            if (hasCtor)
                            {
                                errorHandler.ReportError(new NodeValidationError("Constructor overloads are not supported.", memberNode));
                            }

                            hasCtor = true;
                        }

                        continue;
                    }

                    if (memberNode.NodeType == ParseNodeType.OperatorDeclaration)
                    {
                        // Operators don't have a name
                        continue;
                    }

                    string name = memberNode.Name;

                    if (memberNames.ContainsKey(name))
                    {
                        errorHandler.ReportError(new NodeValidationError("Duplicate-named member. Method overloads are not supported.", memberNode));
                    }

                    memberNames[name] = null;

                    string nameToValidate = name;
                    bool preserveCase = false;
                    AttributeNode nameAttribute = AttributeNode.FindAttribute(memberNode.Attributes, "ScriptName");

                    if (nameAttribute != null && nameAttribute.Arguments.Count != 0)
                    {
                        foreach (ParseNode argNode in nameAttribute.Arguments)
                            if (argNode.NodeType == ParseNodeType.Literal)
                            {
                                nameToValidate = (string)((LiteralNode)argNode).Value;
                            }
                            else if (argNode.NodeType == ParseNodeType.BinaryExpression)
                            {
                                if (string.CompareOrdinal(((NameNode)((BinaryExpressionNode)argNode).LeftChild).Name,
                                        "PreserveCase") == 0)
                                {
                                    preserveCase = (bool)((LiteralNode)((BinaryExpressionNode)argNode).RightChild)
                                        .Value;
                                }
                            }
                    }

                    if (Utility.IsKeyword(nameToValidate, /* testCamelCase */ preserveCase == false))
                    {
                        errorHandler.ReportError(new NodeValidationError("Invalid member name. Member names should not use keywords.", memberNode));
                    }

                    if (hasCodeMembers == false)
                    {
                        hasCodeMembers = memberNode.NodeType == ParseNodeType.PropertyDeclaration ||
                                         memberNode.NodeType == ParseNodeType.MethodDeclaration ||
                                         memberNode.NodeType == ParseNodeType.EventDeclaration ||
                                         memberNode.NodeType == ParseNodeType.IndexerDeclaration;
                        codeMemberNode = memberNode;
                    }
                }
            }

            return true;
        }
    }
}
