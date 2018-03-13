using System.Runtime.Serialization;

namespace XmlContractSerialization
{
    [DataContract]
    public class Parameter
    {
        [DataMember]
        public string Name { get; set; }
    }
}
