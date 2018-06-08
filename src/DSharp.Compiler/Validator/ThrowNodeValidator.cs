// ThrowNodeValidator.cs
// Script#/Core/Compiler
// This source code is subject to terms and conditions of the Apache License, Version 2.0.
//

using DSharp.Compiler.CodeModel;
using DSharp.Compiler.CodeModel.Statements;

namespace DSharp.Compiler.Validator
{
    internal sealed class ThrowNodeValidator : IParseNodeValidator
    {
        bool IParseNodeValidator.Validate(ParseNode node, CompilerOptions options, IErrorHandler errorHandler)
        {
            ThrowNode throwNode = (ThrowNode) node;

            if (throwNode.Value == null)
            {
                errorHandler.ReportError("Throw statements must specify an exception object.",
                    throwNode.Token.Location);

                return false;
            }

            return true;
        }
    }
}