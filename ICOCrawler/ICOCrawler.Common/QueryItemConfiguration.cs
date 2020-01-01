using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ICOCrawler.Common
{
    [DataContract]
    [KnownType(typeof(QueryItem))]
    public class QueryItemConfiguration
    {
        [DataMember]
        public IEnumerable<QueryItem> QueryItems { get; set; }
    }
}
