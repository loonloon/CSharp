using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Xml;

namespace XmlContractSerialization
{
    class Program
    {
        static void Main(string[] args)
        {
            var filePath = string.Empty;
            var executablePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);


            if (executablePath != null)
            {
                filePath = Path.Combine(executablePath, "parameters.xml");
            }

            WriteXml(filePath, new ParameterConfiguration
            {
                Parameters = new List<Parameter>
                {
                    new Parameter { Name = "Pressure" }
                }
            });

            ReadXml(filePath);
        }

        public static void ReadXml(string fileName)
        {
            var parameterConfiguration = SerializationUtil.DeserializeDataContractFromFile<ParameterConfiguration>(fileName);
        }

        public static void WriteXml<T>(string fileName, T configuration)
        {
            var dataContractSerializer = new DataContractSerializer(typeof(T));
            var xmlWriterSettings = new XmlWriterSettings { Indent = true };

            using (var xmlWriter = XmlWriter.Create(fileName, xmlWriterSettings))
            {
                dataContractSerializer.WriteObject(xmlWriter, configuration);
            }
        }
    }
}
