using System.Linq;

namespace loonloon.Grpc.Auth.Portfolio.Service.Protos
{
    public partial class Portfolio
    {
        public static Portfolio FromRepositoryModel(Data.Models.Portfolio source)
        {
            if (source is null)
            {
                return null;
            }

            var target = new Portfolio
            {
                Id = source.Id,
                TraderId = source.TraderId.ToString(),
            };

            target.Items.AddRange(source.Items.Select(PortfolioItem.FromRepositoryModel));

            return target;
        }
    }
}
