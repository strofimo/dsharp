// IErrorHandler.cs
// Script#/Core/Compiler
// This source code is subject to terms and conditions of the Apache License, Version 2.0.
//

using System;

namespace DSharp.Compiler
{
    public interface IErrorHandler
    {
        [Obsolete("This is being replaced with the IError variant", false)]
        void ReportError(string errorMessage, string location);

        void ReportError(IError error);
    }
}
