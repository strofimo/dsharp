// ClassSymbol.cs
// Script#/Core/ScriptSharp
// Copyright (c) Nikhil Kothari.
// Copyright (c) Microsoft Corporation.
// This source code is subject to terms and conditions of the Microsoft 
// Public License. A copy of the license can be found in License.txt.
//

namespace DSharp.Compiler.ScriptModel.Symbols
{
    internal sealed class GenericTypeSymbol : TypeSymbol
    {
        public GenericTypeSymbol(int genericArgumentIndex, NamespaceSymbol parent)
            : base(SymbolType.GenericParameter, "<T>", parent)
        {
            GenericArgumentIndex = genericArgumentIndex;
        }

        public int GenericArgumentIndex { get; }
    }
}