// CodeModelBuilder.cs
// Script#/Core/Compiler
// This source code is subject to terms and conditions of the Apache License, Version 2.0.
//

using DSharp.Compiler.CodeModel.Types;

namespace DSharp.Compiler.CodeModel
{
    internal interface ICodeModelBuilder
    {
        CompilationUnitNode BuildCodeModel(IStreamSource source);
    }
}
