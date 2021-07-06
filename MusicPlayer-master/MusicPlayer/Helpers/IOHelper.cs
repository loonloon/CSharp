using System;
using System.IO;
using System.Reflection;

namespace MusicPlayer.Helpers
{
    /// <summary>
    /// Class with helper methods for IO related tasks.
    /// </summary>
    public static class IOHelper
    {
        /// <summary>
        /// Reads an embedded resource as text.
        /// </summary>
        /// <param name="resourceName">The resource name.</param>
        /// <returns>The content as text.</returns>
        public static string ReadEmbeddedResourceText(string resourceName)
        {
            try
            {
                var assembly = Assembly.GetExecutingAssembly();
                using (var stream = assembly.GetManifestResourceStream(resourceName))
                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Could not read embedded resource: " + resourceName);
                return null;
            }
        }
    }
}
