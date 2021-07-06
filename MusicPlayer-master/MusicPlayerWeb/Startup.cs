using System;
using System.IO;
using System.Linq;
using System.Reflection;
using CefSharp;
using MusicPlayer;

namespace MusicPlayerWeb
{
    /// <summary>
    /// Class that allows the application to start when the browser process fails (log problem).
    /// </summary>
    public static class Startup
    {
        /// <summary>
        /// Will start the browser process.
        /// </summary>
        public static void Start()
        {
            var dir = AppDomain.CurrentDomain.BaseDirectory;
            var missingDeps = DependencyChecker.CheckDependencies(true, false, dir, string.Empty, Path.Combine(dir, "CefSharp.BrowserSubprocess.exe"));

            if (missingDeps?.Count > 0)
            {
                Logger.LogInfo($"Missing dependancies:{missingDeps.Aggregate((r, s) => $"{r}, {s}")}");
            }

            var directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            directory = directory != null && directory.EndsWith("\\") ? directory : $"{directory}\\";

            var settings = new CefSettings();
            settings.RegisterScheme(new CefCustomScheme
            {
                SchemeName = "custom",
                SchemeHandlerFactory = new SchemeHandlerFactory(directory)
            });

            settings.CefCommandLineArgs.Add("disable-gpu", "1");
            Cef.Initialize(settings, true, null);
        }
    }
}
