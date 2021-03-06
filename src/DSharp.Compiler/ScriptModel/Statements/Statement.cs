// Statement.cs
// Script#/Core/Compiler
// This source code is subject to terms and conditions of the Apache License, Version 2.0.
//

namespace DSharp.Compiler.ScriptModel.Statements
{
    internal abstract class Statement
    {
        protected Statement(StatementType type)
        {
            Type = type;
        }

        public virtual bool RequiresThisContext => false;

        public StatementType Type { get; }
    }
}