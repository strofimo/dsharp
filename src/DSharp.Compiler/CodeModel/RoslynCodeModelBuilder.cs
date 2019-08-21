using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DSharp.Compiler.CodeModel.Attributes;
using DSharp.Compiler.CodeModel.Expressions;
using DSharp.Compiler.CodeModel.Names;
using DSharp.Compiler.CodeModel.Tokens;
using DSharp.Compiler.CodeModel.Types;
using DSharp.Compiler.ScriptModel.Expressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DSharp.Compiler.CodeModel
{
    internal class RoslynCodeModelBuilder : ICodeModelBuilder
    {
        private readonly MetadataReference[] metadataReferences;
        private readonly CSharpParseOptions parseOptions;

        public RoslynCodeModelBuilder(CompilerOptions options)
        {
            metadataReferences = options.References
                .Select(reference => MetadataReference.CreateFromFile(reference))
                .ToArray();

            parseOptions = new CSharpParseOptions(
                LanguageVersion.CSharp3,
                documentationMode: options.EnableDocComments ? DocumentationMode.Parse : DocumentationMode.None,
                kind: SourceCodeKind.Regular,
                preprocessorSymbols: options.Defines);
        }

        public CompilationUnitNode BuildCodeModel(IStreamSource source)
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(LoadText(source), options: parseOptions);
            var root = syntaxTree.GetCompilationUnitRoot();

            List<ParseNode> parsedUsings = ParseUsings(root.Usings);
            List<ParseNode> parsedAttributes = ParseAttributes(root.AttributeLists);
            List<ParseNode> parsedNamespaceMembers = ParseMembers(root);

            return new CompilationUnitNode(
                token: null, //Should be BOF
                externAliases: new ParseNodeList(), //Not actually sure what goes in here
                usingClauses: new ParseNodeList(parsedUsings),
                attributes: new ParseNodeList(parsedAttributes),
                members: new ParseNodeList(parsedNamespaceMembers));
        }

        private List<ParseNode> ParseMembers(SyntaxNode node)
        {
            List<ParseNode> parsedMembers = new List<ParseNode>();

            foreach (var member in node.ChildNodes())
            {
                switch(member)
                {
                    case NamespaceDeclarationSyntax namespaceDeclaration:
                        NamespaceNode namespaceNode = ParseNamespace(namespaceDeclaration);
                        parsedMembers.Add(namespaceNode);
                        break;
                    case ClassDeclarationSyntax classDeclarationSyntax:
                        CustomTypeNode classNode = ParseClassDefinition(classDeclarationSyntax);
                        parsedMembers.Add(classNode);
                        break;
                    case TypeDeclarationSyntax typeDeclarationSyntax:
                        break;
                    default:
                        break;
                }
            }

            return parsedMembers;
        }

        private CustomTypeNode ParseClassDefinition(ClassDeclarationSyntax classDeclarationSyntax)
        {
            var attributes = new ParseNodeList(ParseAttributes(classDeclarationSyntax.AttributeLists));
            var name = CreateAtomicName(classDeclarationSyntax.Identifier);
            var modifiers = ParseModifiers(classDeclarationSyntax.Modifiers);
            var classNode = new CustomTypeNode(
                null,
                TokenType.Class,
                attributes,
                modifiers, // Parse actual modifiers
                name,
                typeParameters: null,
                baseTypes: null,
                constraintClauses: null,
                members: null);
            return classNode;
        }

        private Modifiers ParseModifiers(SyntaxTokenList modifiers)
        {
            var parsedModifier = Modifiers.None;
            foreach (var modifier in modifiers)
            {
                switch (modifier.Kind())
                {
                    case SyntaxKind.PublicKeyword:
                        parsedModifier |= Modifiers.Public;
                        break;
                    case SyntaxKind.InternalKeyword:
                        parsedModifier |= Modifiers.Internal;
                        break;
                    case SyntaxKind.PrivateKeyword:
                        parsedModifier |= Modifiers.Private;
                        break;
                    case SyntaxKind.ProtectedKeyword:
                        parsedModifier |= Modifiers.Protected;
                        break;
                    case SyntaxKind.PartialKeyword:
                        parsedModifier |= Modifiers.Partial;
                        break;
                    case SyntaxKind.AbstractKeyword:
                        parsedModifier |= Modifiers.Abstract;
                        break;
                    case SyntaxKind.StaticKeyword:
                        parsedModifier |= Modifiers.Static;
                        break;
                    case SyntaxKind.SealedKeyword:
                        parsedModifier |= Modifiers.Sealed;
                        break;
                    default:
                        throw new Exception();
                }
            }

            return parsedModifier;
        }

        private NamespaceNode ParseNamespace(NamespaceDeclarationSyntax namespaceDeclaration)
        {
            var nameNode = ParseNameNode(namespaceDeclaration.Name);
            var usings = new ParseNodeList(ParseUsings(namespaceDeclaration.Usings));
            var members = ParseMembers(namespaceDeclaration);
            var namespaceNode = new NamespaceNode(null, nameNode, new ParseNodeList(), usings, new ParseNodeList(members));
            return namespaceNode;
        }

        private List<ParseNode> ParseAttributes(IEnumerable<AttributeListSyntax> attributeLists)
        {
            List<ParseNode> attributes = new List<ParseNode>();
            foreach (AttributeListSyntax attributeList in attributeLists)
            {
                List<AttributeNode> attributeNodes = new List<AttributeNode>();
                var target = attributeList.Target;
                foreach(var attribute in attributeList.Attributes)
                {
                    var name = ParseNameNode(attribute.Name);
                    
                    attributeNodes.Add(new AttributeNode(name, ParseAttributesArguments(attribute.ArgumentList)));
                }

                var attributeBlockNode = new AttributeBlockNode(null, new ParseNodeList(attributeNodes));
            }

            return attributes;
        }

        private static ExpressionListNode ParseAttributesArguments(AttributeArgumentListSyntax argumentList)
        {
            List<ParseNode> expressions = new List<ParseNode>();

            foreach (var argument in argumentList.Arguments)
            {
                var expression = ParseExpression(argument.Expression);
                expressions.Add(expression);
            }

            return new ExpressionListNode(null, new ParseNodeList(expressions));
        }

        private static ParseNode ParseExpression(ExpressionSyntax expressionSyntax)
        {
            switch(expressionSyntax)
            {
                case LiteralExpressionSyntax literalExpression:
                    return new LiteralNode(ParseLiteralExpressionAsToken(literalExpression));
                case MemberAccessExpressionSyntax memberAccessExpressionSyntax:
                    return new BinaryExpressionNode(ParseExpression(memberAccessExpressionSyntax.Expression), TokenType.Dot, ParseSimpleName(memberAccessExpressionSyntax.Name));
                case IdentifierNameSyntax identifierNameSyntax:
                    return ParseIdentifier(identifierNameSyntax);
                default:
                    throw new Exception();
            }
        }

        private static LiteralToken ParseLiteralExpressionAsToken(LiteralExpressionSyntax literalExpression)
        {
            var position = GetTokenPosition(literalExpression.Token, out string path);

            switch (literalExpression.Kind())
            {
                case SyntaxKind.StringLiteralExpression:
                    return new StringToken(literalExpression.Token.ValueText, path, position);
                case SyntaxKind.NumericLiteralExpression:
                    switch(literalExpression.Token.Value)
                    {
                        case uint uint32:
                            return new UIntToken(uint32, path, position);
                        case int int32:
                            return new IntToken(int32, path, position);
                        case ulong uint64:
                            return new ULongToken(uint64, path, position);
                        case long int64:
                            return new LongToken(int64, path, position);
                        default:
                            throw new Exception();
                    }
                case SyntaxKind.FalseLiteralExpression:
                case SyntaxKind.TrueLiteralExpression:
                    return new BooleanToken((bool)literalExpression.Token.Value, path, position);
                case SyntaxKind.NullLiteralExpression:
                    return new NullToken(path, position);
                default:
                    throw new Exception();
            }
        }

        private static List<ParseNode> ParseUsings(IEnumerable<UsingDirectiveSyntax> usings)
        {
            List<ParseNode> parsedUsings = new List<ParseNode>();

            foreach (var usingNode in usings)
            {
                NameNode namespaceNameNode = ParseNameNode(usingNode.Name);
                if (usingNode.Alias != null)
                {
                    var name = CreateAtomicName(usingNode.Alias.Name.Identifier);
                    var usingAlias = new UsingAliasNode(null, name, namespaceNameNode);
                    parsedUsings.Add(usingAlias);
                }
                else
                {
                    var usingNamespace = new UsingNamespaceNode(null, namespaceNameNode);
                    parsedUsings.Add(usingNamespace);
                }
            }

            return parsedUsings;
        }

        private static NameNode ParseNameNode(NameSyntax nameSyntax)
        {
            if (nameSyntax is QualifiedNameSyntax)
            {
                List<ParseNode> parseNodes = new List<ParseNode>();
                NameSyntax current = nameSyntax;
                while (current != null)
                {
                    if (current is QualifiedNameSyntax qualifiedNameSyntax)
                    {
                        var nameNode = ParseSimpleName(qualifiedNameSyntax.Right);
                        parseNodes.Add(nameNode);
                        current = qualifiedNameSyntax.Left;
                        continue;
                    }

                    //Must be an identifer
                    var identifier = ParseIdentifier(current as IdentifierNameSyntax);
                    parseNodes.Add(identifier);
                    break;
                }

                return new MultiPartNameNode(null, new ParseNodeList(parseNodes.Reverse<ParseNode>()));
            }
            else if (nameSyntax is IdentifierNameSyntax identifierNameSyntax)
            {
                return ParseIdentifier(identifierNameSyntax);
            }

            throw new Exception("We can't get here?!");
        }

        private static NameNode ParseSimpleName(SimpleNameSyntax simpleNameSyntax)
        {
            return CreateAtomicName(simpleNameSyntax.Identifier);
        }

        private static NameNode ParseIdentifier(IdentifierNameSyntax identifierNameSyntax)
        {
            return CreateAtomicName(identifierNameSyntax.Identifier);
        }

        private static AtomicNameNode CreateAtomicName(SyntaxToken syntaxToken)
        {
            var position = GetTokenPosition(syntaxToken, out string path);
            var nameNode = new Parser.Name(syntaxToken.ValueText);
            return new AtomicNameNode(new Tokens.IdentifierToken(nameNode, nameNode.Text.StartsWith("@"), path, position));
        }

        private static Parser.BufferPosition GetTokenPosition(SyntaxToken syntaxToken, out string path)
        {
            var location = syntaxToken.GetLocation();
            var lineSpan = location.GetLineSpan();
            path = location.SourceTree.FilePath;
            return new Parser.BufferPosition(lineSpan.StartLinePosition.Line, lineSpan.StartLinePosition.Character, 0);
        }

        private string LoadText(IStreamSource source)
        {
            Stream stream = source.GetStream();
            string text = string.Empty;
            if (stream != null)
            {
                StreamReader reader = new StreamReader(stream);
                text = reader.ReadToEnd();

                source.CloseStream(stream);
            }

            return text;
        }

        private class LocalSyntaxWalker : CSharpSyntaxWalker
        {
            public LocalSyntaxWalker(SyntaxWalkerDepth depth = SyntaxWalkerDepth.Node)
                : base(depth)
            {
            }

            public override void VisitNamespaceDeclaration(NamespaceDeclarationSyntax node)
            {
                base.VisitNamespaceDeclaration(node);
            }
        }
    }
}
