using System;
using System.Collections.Generic;

namespace loonloon.Grpc.Auth.Portfolio.Data.Models
{
    public class Portfolio
    {
        public int Id { get; set; }
        public Guid TraderId { get; set; }
        public List<PortfolioItem> Items { get; set; }
    }
}
