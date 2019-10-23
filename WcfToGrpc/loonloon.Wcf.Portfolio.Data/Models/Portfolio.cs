using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace loonloon.Wcf.Portfolio.Data.Models
{
    [DataContract]
    public class Portfolio
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public Guid TraderId { get; set; }

        [DataMember]
        public List<PortfolioItem> Items { get; set; }
    }
}
