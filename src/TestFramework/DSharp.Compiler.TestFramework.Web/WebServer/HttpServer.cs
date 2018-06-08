// HttpServer.cs
// Script#/Tools/Testing
// This source code is subject to terms and conditions of the Apache License, Version 2.0.
//

using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace DSharp.Compiler.TestFramework.Web.WebServer {

    internal abstract class HttpServer {

        private int port;
        private Action<HttpMessage> getHandler;
        private Action<HttpMessage> postHandler;

        private bool alive;

        protected void Initialize(Action<HttpMessage> getHandler, Action<HttpMessage> postHandler) {
            this.getHandler = getHandler;
            this.postHandler = postHandler;
        }

        private void Run() {
            TcpListener listener = new TcpListener(new IPEndPoint(IPAddress.Loopback, port));
            listener.Start();

            while (alive) {
                try {
                    if (listener.Pending()) {
                        TcpClient client = listener.AcceptTcpClient();

                        HttpMessage message = new HttpMessage(this, getHandler, postHandler);
                        message.ProcessClient(client);
                    }
                }
                catch {
                }

                Thread.Sleep(1);
            }

            listener.Stop();
        }

        public void Start(int port) {
            if (alive) {
                throw new InvalidOperationException();
            }

            alive = true;
            this.port = port;

            Thread mainThread = new Thread(Run);
            mainThread.Start();
        }

        public void Stop() {
            alive = false;
        }
    }
}
