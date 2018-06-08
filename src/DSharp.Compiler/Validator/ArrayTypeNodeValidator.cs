// ArrayTypeNodeValidator.cs
// Script#/Core/Compiler
// This source code is subject to terms and conditions of the Apache License, Version 2.0.
//

using DSharp.Compiler.CodeModel;
using DSharp.Compiler.CodeModel.Types;

namespace DSharp.Compiler.Validator
{
    internal sealed class ArrayTypeNodeValidator : IParseNodeValidator
    {
        bool IParseNodeValidator.Validate(ParseNode node, CompilerOptions options, IErrorHandler errorHandler)
        {
            ArrayTypeNode typeNode = (ArrayTypeNode) node;

            if (typeNode.Rank != 1)
            {
                errorHandler.ReportError("Only single dimensional arrays are supported.",
                    typeNode.Token.Location);
            }

            return true;
        }
    }
}