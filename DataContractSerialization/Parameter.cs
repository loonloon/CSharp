using System.Runtime.Serialization;

namespace DataContractSerialization
{
    [DataContract]
    public class Parameter
    {
        [DataMember]
        public string Name { get; set; }
    }
}
