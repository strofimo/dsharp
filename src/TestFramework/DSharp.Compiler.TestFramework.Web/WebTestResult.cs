// WebTestResult.cs
// Script#/Tools/Testing
// This source code is subject to terms and conditions of the Apache License, Version 2.0.
//

namespace DSharp.Compiler.TestFramework.Web {

    public sealed class WebTestResult {

        private readonly bool succeeded;
        private readonly bool timedOut;
        private readonly string log;

        internal WebTestResult() {
            timedOut = true;
        }

        internal WebTestResult(bool succeeded, string log) {
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

        public bool TimedOut {
            get {
                return timedOut;
            }
        }
    }
}
