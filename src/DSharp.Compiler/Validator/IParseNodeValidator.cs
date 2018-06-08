// IParseNodeValidator.cs
// Script#/Core/Compiler
// This source code is subject to terms and conditions of the Apache License, Version 2.0.
//

using DSharp.Compiler.CodeModel;

namespace DSharp.Compiler.Validator
{
    /// <summary>
    ///     Defines a parse node validator
    /// </summary>
    internal interface IParseNodeValidator
    {
        bool Validate(ParseNode node, CompilerOptions options, IErrorHandler errorHandler);
    }
}