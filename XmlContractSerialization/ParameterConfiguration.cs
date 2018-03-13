using System.Collections.Generic;
using System.Runtime.Serialization;

namespace XmlContractSerialization
{
    [DataContract]
    [KnownType(typeof(Parameter))]
    public class ParameterConfiguration
    {
        [DataMember]
        public IEnumerable<Parameter> Parameters { get; set; }
    }
}
