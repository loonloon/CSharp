using CefSharp;
using System;
using System.IO;
using System.Linq;

namespace MusicPlayerWeb
{
    /// <summary>
    /// Class to handle file requests from the browser.
    /// </summary>
    internal class SchemeHandlerFactory : ISchemeHandlerFactory
    {
        /// <summary>
        /// The name of the customer scheme.
        /// </summary>
        public const string SchemeName = "custom";

        /// <summary>
        /// The application root folder.
        /// </summary>
        private readonly string _root;

        /// <summary>
        /// Initializes a new instance of the <see cref="SchemeHandlerFactory" /> class.
        /// </summary>
        /// <param name="root"></param>
        public SchemeHandlerFactory(string root)
        {
            _root = root;
        }

        /// <summary>
        /// Creates a resourceHandler for the request.
        /// </summary>
        /// <param name="browser"></param>
        /// <param name="frame"></param>
        /// <param name="schemeName"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public IResourceHandler Create(IBrowser browser, IFrame frame, string schemeName, IRequest request)
        {
            var uri = new Uri(request.Url);
            var fileName = uri.AbsolutePath;
            var resource = $"{_root}build{fileName.Replace('/', '\\')}";
            var fileExtension = Path.GetExtension(fileName);
            return ResourceHandler.FromFilePath(resource, new[] { ".woff", ".woff2", ".ttf" }.Contains(fileExtension) ?
                "text/html" : ResourceHandler.GetMimeType(fileExtension));
        }
    }
}
