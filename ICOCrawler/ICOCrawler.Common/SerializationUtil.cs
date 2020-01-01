using System.IO;
using System.Runtime.Serialization;
using System.Text;

namespace ICOCrawler.Common
{
    public static class SerializationUtil
    {
        public static T DeserializeDataContract<T>(string xml)
        {
            var value = default(T);
            var dataContractSerializer = new DataContractSerializer(typeof(T));

            using (var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(xml)))
            {
                try
                {
                    value = (T)dataContractSerializer.ReadObject(memoryStream);
                }
                catch (SerializationException)
                {

                }
            }

            return value;
        }

        public static T DeserializeDataContractFromFile<T>(Stream stream)
        {
            T value;

            using (var streamReader = new StreamReader(stream))
            {
                var xml = streamReader.ReadToEnd();
                value = DeserializeDataContract<T>(xml);
            }

            return value;
        }
    }
}
