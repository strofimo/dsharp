using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DSharp.Compiler.CodeModel.Attributes;
using DSharp.Compiler.CodeModel.Expressions;
using DSharp.Compiler.CodeModel.Members;
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
        private readonly CSharpParseOptions parseOptions;

        public RoslynCodeModelBuilder(CompilerOptions options)
        {
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

            var parsedUsings = ParseUsings(root.Usings);
            var parsedAttributes = ParseAttributes(root.AttributeLists);
            var parsedNamespaceMembers = ParseMembers(root);

            return new CompilationUnitNode(
                token: null, //Should be BOF, but does it matter!?
                externAliases: new ParseNodeList(), //Not actually sure what goes in here
                usingClauses: parsedUsings,
                attributes: parsedAttributes,
                members: parsedNamespaceMembers);
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
                        CustomTypeNode classNode = ParseCustomTypeDefinition(TokenType.Class, classDeclarationSyntax);
                        parsedMembers.Add(classNode);
                        break;
                    case InterfaceDeclarationSyntax interfaceDeclarationSyntax:
                        CustomTypeNode interfaceNode = ParseCustomTypeDefinition(TokenType.Interface, interfaceDeclarationSyntax);
                        parsedMembers.Add(interfaceNode);
                        break;
                    case StructDeclarationSyntax structDeclarationSyntax:
                        CustomTypeNode structNode = ParseCustomTypeDefinition(TokenType.Struct, structDeclarationSyntax);
                        parsedMembers.Add(structNode);
                        break;
                    case TypeDeclarationSyntax typeDeclarationSyntax:
                        break;  
                    default:
                        break;
                }
            }

            return parsedMembers;
        }

        private CustomTypeNode ParseCustomTypeDefinition(TokenType type, TypeDeclarationSyntax typeDeclarationSyntax)
        {
            var attributes = new ParseNodeList(ParseAttributes(typeDeclarationSyntax.AttributeLists));
            var name = CreateAtomicName(typeDeclarationSyntax.Identifier);
            var modifiers = ParseModifiers(typeDeclarationSyntax.Modifiers);
            var typeParameters = ParseTypeParameters(typeDeclarationSyntax.TypeParameterList);
            var baseTypes = ParseBaseTypes(typeDeclarationSyntax.BaseList);
            var constraints = ParseConstraints(typeDeclarationSyntax.ConstraintClauses);
            var members = ParseTypeMembers(typeDeclarationSyntax.Members);

            var classNode = new CustomTypeNode(
                null,
                type,
                attributes,
                modifiers, // Parse actual modifiers
                name,
                typeParameters: typeParameters,
                baseTypes: baseTypes,
                constraintClauses: constraints,
                members: members);
            return classNode;
        }

        private ParseNodeList ParseTypeMembers(SyntaxList<MemberDeclarationSyntax> members)
        {
            ParseNodeList parseNodes = new ParseNodeList();

            foreach (var member in members)
            {
                var attributes = ParseAttributes(member.AttributeLists);
                var modifiers = ParseModifiers(member.Modifiers);
                switch(member)
                {
                    case FieldDeclarationSyntax fieldDeclarationSyntax:
                        var field = new FieldDeclarationNode(null, attributes, modifiers, null, null, false);
                        parseNodes.Add(field);
                        break;
                    case BaseFieldDeclarationSyntax baseFieldDeclarationSyntax: //Is this actually a thing?
                        break;
                    case ConstructorDeclarationSyntax constructorDeclarationSyntax:
                        break;
                    case DestructorDeclarationSyntax destructorDeclarationSyntax:
                        break;
                    case IndexerDeclarationSyntax indexerDeclarationSyntax:
                        break;
                    case EventDeclarationSyntax eventDeclarationSyntax:
                        break;
                    case PropertyDeclarationSyntax propertyDeclarationSyntax:
                        break;
                    case MethodDeclarationSyntax methodDeclarationSyntax:
                        MethodDeclarationNode method = ParseMethod(methodDeclarationSyntax, attributes, modifiers);
                        parseNodes.Add(method);
                        break;
                    case OperatorDeclarationSyntax operatorDeclarationSyntax:
                        break;
                    case TypeDeclarationSyntax typeDeclarationSyntax:
                        throw new NotSupportedException();
                    default:
                        throw new Exception("Invalid member!");
                }
            }

            return parseNodes;
        }

        private MethodDeclarationNode ParseMethod(MethodDeclarationSyntax methodDeclarationSyntax, ParseNodeList attributes, Modifiers modifiers)
        {
            var returnType = ParseTypeName(methodDeclarationSyntax.ReturnType);
            var name = CreateAtomicName(methodDeclarationSyntax.Identifier);
            var typeParameters = ParseTypeParameters(methodDeclarationSyntax.TypeParameterList);
            var constraints = ParseConstraints(methodDeclarationSyntax.ConstraintClauses);
            var interfaceType = ParseExplicitInterfaceType(methodDeclarationSyntax);
            var parameters = ParseParameters(methodDeclarationSyntax.ParameterList);

            var method = new MethodDeclarationNode(
                null,
                attributes,
                modifiers,
                returnType: returnType,
                interfaceType: interfaceType,
                name: name,
                typeParameters: typeParameters,
                parameters: parameters,
                constraints: constraints,
                body: null);
            return method;
        }

        private static NameNode ParseExplicitInterfaceType(MethodDeclarationSyntax methodDeclarationSyntax)
        {
            if (methodDeclarationSyntax.ExplicitInterfaceSpecifier != null)
            {
                return ParseNameNode(methodDeclarationSyntax.ExplicitInterfaceSpecifier.Name);
            }

            return null;
        }

        private ParseNodeList ParseParameters(ParameterListSyntax parameterList)
        {
            ParseNodeList parseNodes = new ParseNodeList();

            foreach(var parameter in parameterList.Parameters)
            {
                var attributes = ParseAttributes(parameter.AttributeLists);
                ParameterFlags parameterFlags = ParameterFlags.None; //Parse the modifiers or keywords
                var typeName = ParseTypeName(parameter.Type);
                var name = CreateAtomicName(parameter.Identifier);
                ParameterNode parameterNode = new ParameterNode(
                    null,
                    attributes,
                    parameterFlags,
                    typeName,
                    name);

                parseNodes.Add(parameterNode);
            }

            return parseNodes;
        }

        private static ParseNode ParseTypeName(TypeSyntax typeSyntax)
        {
            if(typeSyntax is IdentifierNameSyntax identifierNameSyntax)
            {
                return CreateAtomicName(identifierNameSyntax.Identifier);
            }

            throw new Exception();
        }

        private static ParseNodeList ParseConstraints(SyntaxList<TypeParameterConstraintClauseSyntax> constraintClauses)
        {
            ParseNodeList parseNodes = new ParseNodeList();

            foreach (var constraintClause in constraintClauses)
            {
                //We don't really care about this.
                var name = ParseIdentifier(constraintClause.Name);
                var constraint = new TypeParameterConstraintNode((AtomicNameNode)name, new ParseNodeList(), false);
                parseNodes.Add(constraint);
            }

            return parseNodes;
        }

        private ParseNodeList ParseBaseTypes(BaseListSyntax baseList)
        {
            ParseNodeList parseNodes = new ParseNodeList();
            if(baseList == null)
            {
                return parseNodes;
            }

            foreach(var type in baseList.Types)
            {
                if(type is SimpleBaseTypeSyntax simpleBaseTypeSyntax)
                {
                    var name = ParseTypeName(simpleBaseTypeSyntax.Type);
                    parseNodes.Add(name);
                }
                else
                {
                    throw new Exception();
                }
            }

            return parseNodes;
        }

        private ParseNodeList ParseTypeParameters(TypeParameterListSyntax typeParameterList)
        {
            ParseNodeList parseNodes = new ParseNodeList();

            if(typeParameterList == null)
            {
                return parseNodes;
            }

            foreach (var typeParameterSyntax in typeParameterList.Parameters)
            {
                var attributes = ParseAttributes(typeParameterSyntax.AttributeLists);
                var name = CreateAtomicName(typeParameterSyntax.Identifier);
                var typeParameter = new TypeParameterNode(attributes, name);

                parseNodes.Add(typeParameter);
            }

            return parseNodes;
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
