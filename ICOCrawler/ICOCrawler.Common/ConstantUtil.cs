using System;
using System.IO;
using System.Reflection;

namespace ICOCrawler.Common
{
    public static class ConstantUtil
    {
        public static string QueryItemConfigFilePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? throw new InvalidOperationException(), "QueryItemConfig.xml");
    }
}
