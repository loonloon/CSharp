using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Xsl;

namespace XmlToHtmlByXslt
{
    class Program
    {
        static void Main(string[] args)
        {
            var xslCompiledTransform = new XslCompiledTransform();
            xslCompiledTransform.Load("sample.xslt");

            var htmlOutput = new StringBuilder();
            var htmlWriter = new StringWriter(htmlOutput);

            // Creating XmlReader object to read XML content    
            using (var reader = XmlReader.Create("sample.xml"))
            {
                // Call Transform() method to create html string and write in TextWriter object.    
                xslCompiledTransform.Transform(reader, null, htmlWriter);
            }

            Console.WriteLine(htmlOutput.ToString());
        }
    }
}
