// HttpMessage.cs
// Script#/Tools/Testing
// This source code is subject to terms and conditions of the Apache License, Version 2.0.
//

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace DSharp.Compiler.TestFramework.Web.WebServer {

    internal sealed class HttpMessage {

        private const int MAXIMUM_POST_SIZE = 10 * 1024 * 1024; // 10MB
        private const int BUFFER_SIZE = 4096;

        private HttpServer server;
        private TcpClient client;

        private readonly Action<HttpMessage> getHandler;
        private readonly Action<HttpMessage> postHandler;

        private string method;
        private string path;
        private Dictionary<string, string> headers;

        private Stream requestStream;
        private Stream responseStream;

        internal HttpMessage(HttpServer server, Action<HttpMessage> getHandler, Action<HttpMessage> postHandler) {
            this.server = server;
            this.getHandler = getHandler;
            this.postHandler = postHandler;
        }

        public string Method {
            get {
                return method;
            }
        }

        public string Path {
            get {
                return path;
            }
        }

        public Stream RequestStream {
            get {
                return requestStream;
            }
        }

        public Stream ResponseStream {
            get {
                return responseStream;
            }
        }

        private void ParseRequest(Stream inputStream) {
            string request = ReadLine(inputStream);

            string[] tokens = request.Split(' ');
            if (tokens.Length != 3) {
                throw new Exception("Invalid HTTP request");
            }

            method = tokens[0].ToUpper();
            path = tokens[1];
        }

        private void ParseRequestHeaders(Stream inputStream) {
            headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            string line;
            while ((line = ReadLine(inputStream)) != null) {
                if (line.Equals("")) {
                    // Finished reading headers
                    return;
                }

                int separator = line.IndexOf(':');
                if (separator == -1) {
                    throw new Exception("Invalid HTTP header: " + line);
                }

                string name = line.Substring(0, separator);
                int pos = separator + 1;
                while ((pos < line.Length) && (line[pos] == ' ')) {
                    // Strip any spaces
                    pos++;
                }

                string value = line.Substring(pos, line.Length - pos);
                if (string.IsNullOrEmpty(value)) {
                    throw new Exception("Invalid HTTP header: " + line);
                }

                headers[name] = value;
            }
        }

        private void ParseRequestBody(Stream inputStream) {
            MemoryStream ms = new MemoryStream();

            if (headers.TryGetValue("Content-Length", out string contentLengthValue) &&
                int.TryParse(contentLengthValue, out int contentLength)) {
                if (contentLength > MAXIMUM_POST_SIZE) {
                    throw new Exception("Request is too large");
                }

                int bytesToRead = contentLength;

                byte[] buffer = new byte[BUFFER_SIZE];
                while (bytesToRead > 0) {
                    int bytesRead = inputStream.Read(buffer, 0, Math.Min(BUFFER_SIZE, bytesToRead));
                    if (bytesRead == 0) {
                        if (bytesToRead == 0) {
                            break;
                        }
                        else {
                            throw new Exception("Client disconnected");
                        }
                    }

                    bytesToRead -= bytesRead;
                    ms.Write(buffer, 0, bytesRead);
                }
                ms.Seek(0, SeekOrigin.Begin);
            }

            requestStream = ms;
        }

        public void ProcessClient(TcpClient client) {
            this.client = client;

            Thread messageThread = new Thread(Run);
            messageThread.Start();
        }

        private string ReadLine(Stream inputStream) {
            int nextByte;

            string data = "";
            while (true) {
                nextByte = inputStream.ReadByte();

                if (nextByte == '\n') {
                    break;
                }
                if (nextByte == '\r') {
                    continue;
                }
                if (nextByte == -1) {
                    Thread.Sleep(1);
                    continue;
                };

                data += Convert.ToChar(nextByte);
            }

            return data;
        }

        private void Run() {
            NetworkStream tcpStream = client.GetStream();

            BufferedStream tcpReadStream = new BufferedStream(tcpStream);
            BufferedStream tcpSendStream = new BufferedStream(tcpStream);

            responseStream = tcpSendStream;

            try {
                ParseRequest(tcpReadStream);
                ParseRequestHeaders(tcpReadStream);

                bool processed = false;
                if (method.Equals("GET")) {
                    if (getHandler != null) {
                        getHandler(this);

                        processed = true;
                    }
                }
                else if (method.Equals("POST")) {
                    if (postHandler != null) {
                        ParseRequestBody(tcpReadStream);
                        postHandler(this);

                        processed = true;
                    }
                }

                if (processed == false) {
                    WriteStatus(HttpStatusCode.MethodNotAllowed);
                }
            }
            catch (Exception) {
                WriteStatus(HttpStatusCode.InternalServerError);
            }

            responseStream.Flush();

            client.Close();
        }

        public void WriteContent(string content, string contentType) {
            StreamWriter writer = new StreamWriter(responseStream);

            writer.WriteLine("HTTP/1.0 200 OK");
            writer.WriteLine("Content-Type: " + contentType);
            writer.WriteLine("Connection: close");
            writer.WriteLine("");
            writer.Write(content);

            writer.Flush();
        }

        public void WriteFile(string path, string contentType) {
            StreamWriter writer = new StreamWriter(responseStream);

            writer.WriteLine("HTTP/1.0 200");
            writer.WriteLine("Content-Type: " + contentType);
            writer.WriteLine("Connection: close");
            writer.WriteLine("");
            writer.Flush();

            using (Stream fileStream = File.OpenRead(path)) {
                byte[] buffer = new byte[BUFFER_SIZE];

                int bytesRead = 0;
                do {
                    bytesRead = fileStream.Read(buffer, 0, BUFFER_SIZE);
                    if (bytesRead != 0) {
                        responseStream.Write(buffer, 0, bytesRead);
                    }
                }
                while (bytesRead != 0);
            }
        }

        public void WriteStatus(HttpStatusCode statusCode) {
            StreamWriter writer = new StreamWriter(responseStream);

            writer.WriteLine("HTTP/1.0 " + (int)statusCode);
            writer.WriteLine("Connection: close");
            writer.WriteLine("");

            writer.Flush();
        }
    }
}
