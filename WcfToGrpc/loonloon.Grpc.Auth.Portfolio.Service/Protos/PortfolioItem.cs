using System;

namespace loonloon.Grpc.Auth.Portfolio.Service.Protos
{
    public partial class PortfolioItem
    {
        public static PortfolioItem FromRepositoryModel(Data.Models.PortfolioItem source)
        {
            if (source is null)
            {
                return null;
            }

            return new PortfolioItem
            {
                Id = source.Id,
                ShareId = source.ShareId,
                Holding = source.Holding,
                Cost = Convert.ToDouble(source.Cost)
            };
        }
    }
}
