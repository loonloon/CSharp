using System;
using System.IO;
using System.Runtime.Serialization;
using System.Text;

namespace DataContractSerialization
{
    public class SerializationUtil
    {
        public static T DeserializeDataContractFromFile<T>(string xmlFile)
        {
            if (!File.Exists(xmlFile))
            {
                return default(T);
            }

            var xml = File.ReadAllText(xmlFile);
            return DeserializeDataContract<T>(xml);
        }

        public static T DeserializeDataContract<T>(string xml)
        {
            if (string.IsNullOrEmpty(xml))
            {
                throw new ArgumentNullException(nameof(xml));
            }

            var value = default(T);

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml)))
            {
                var serializer = new DataContractSerializer(typeof(T));

                try
                {
                    value = (T)serializer.ReadObject(stream);
                }
                catch (Exception)
                {
                    // ignored
                }
            }

            return value;
        }
    }
}
