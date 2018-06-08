// EventDeclarationNodeValidator.cs
// Script#/Core/Compiler
// This source code is subject to terms and conditions of the Apache License, Version 2.0.
//

using DSharp.Compiler.CodeModel;
using DSharp.Compiler.CodeModel.Members;

namespace DSharp.Compiler.Validator
{
    internal sealed class EventDeclarationNodeValidator : IParseNodeValidator
    {
        bool IParseNodeValidator.Validate(ParseNode node, CompilerOptions options, IErrorHandler errorHandler)
        {
            EventDeclarationNode eventNode = (EventDeclarationNode) node;

            if ((eventNode.Modifiers & Modifiers.Static) == 0 &&
                (eventNode.Modifiers & Modifiers.New) != 0)
            {
                errorHandler.ReportError("The new modifier is not supported on instance members.",
                    eventNode.Token.Location);

                return false;
            }

            return true;
        }
    }
}