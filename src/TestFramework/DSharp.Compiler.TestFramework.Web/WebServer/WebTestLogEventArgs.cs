// WebTestLogEventArgs.cs
// Script#/Tools/Testing
// This source code is subject to terms and conditions of the Apache License, Version 2.0.
//

using System;

namespace DSharp.Compiler.TestFramework.Web.WebServer {

    internal sealed class WebTestLogEventArgs : EventArgs {

        private readonly bool succeeded;
        private readonly string log;

        internal WebTestLogEventArgs(bool succeeded, string log) {
            this.succeeded = succeeded;
            this.log = log;
        }

        public string Log {
            get {
                return log;
            }
        }

        public bool Succeeded {
            get {
                return succeeded;
            }
        }
    }
}
