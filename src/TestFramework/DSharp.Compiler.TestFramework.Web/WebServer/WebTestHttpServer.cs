// WebTestHttpServer.cs
// Script#/Tools/Testing
// This source code is subject to terms and conditions of the Apache License, Version 2.0.
//

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace DSharp.Compiler.TestFramework.Web.WebServer {

    internal sealed class WebTestHttpServer : HttpServer {

        private readonly Uri baseUri;
        private readonly string[] contentRoots;

        private readonly Dictionary<string, Tuple<string, string>> registeredContent;

        public WebTestHttpServer(string[] contentRoots) {
            this.contentRoots = contentRoots;
            baseUri = new Uri("http://localhost/", UriKind.Absolute);

            Initialize(HandleGetRequest, HandlePostRequest);

            registeredContent =
                new Dictionary<string, Tuple<string, string>>(StringComparer.OrdinalIgnoreCase);
        }

        public event EventHandler<WebTestLogEventArgs> Log;

        private string GetContentType(string path) {
            string extension = Path.GetExtension(path).ToLowerInvariant();

            if ((string.CompareOrdinal(extension, ".htm") == 0) ||
                (string.CompareOrdinal(extension, ".html") == 0)) {
                return "text/html";
            }
            else if (string.CompareOrdinal(extension, ".js") == 0) {
                return "text/javascript";
            }
            else if (string.CompareOrdinal(extension, ".css") == 0) {
                return "text/css";
            }
            else if (string.CompareOrdinal(extension, ".png") == 0) {
                return "image/png";
            }

            return "text/plain";
        }

        private string GetResolvedPath(string urlPath, out string cleanedUrlPath) {
            Uri relativeUri = new Uri(urlPath, UriKind.Relative);
            Uri resolvedUri = new Uri(baseUri, relativeUri);

            // Get the cleaned up path with the leading slash trimmed off
            cleanedUrlPath = resolvedUri.LocalPath;
            return resolvedUri.LocalPath.Substring(1);
        }

        private void HandleGetRequest(HttpMessage message) {
            string path = GetResolvedPath(message.Path, out string urlPath);

            if (registeredContent.TryGetValue(urlPath, out Tuple<string, string> content)) {
                message.WriteContent(content.Item1, content.Item2);
                return;
            }

            foreach (string contentRoot in contentRoots) {
                string possiblePath = Path.Combine(contentRoot, path);
                if (File.Exists(possiblePath)) {
                    message.WriteFile(possiblePath, GetContentType(possiblePath));
                    return;
                }
            }

            message.WriteStatus(HttpStatusCode.NotFound);
        }

        private void HandlePostRequest(HttpMessage message) {
            string path = message.Path;

            bool? success = null;
            if (string.CompareOrdinal(path, "/log/success") == 0) {
                success = true;
            }
            else if (string.CompareOrdinal(path, "/log/failure") == 0) {
                success = false;
            }

            if (success.HasValue) {
                string log = new StreamReader(message.RequestStream).ReadToEnd();
                message.WriteStatus(HttpStatusCode.NoContent);

                if (Log != null) {
                    WebTestLogEventArgs logEvent = new WebTestLogEventArgs(success.Value, log);
                    Log(this, logEvent);
                }

                return;
            }

            message.WriteStatus(HttpStatusCode.NotFound);
        }

        public void RegisterContent(string path, string data, string contentType) {
            Tuple<string, string> content = new Tuple<string, string>(data, contentType);
            registeredContent[path] = content;
        }
    }
}
