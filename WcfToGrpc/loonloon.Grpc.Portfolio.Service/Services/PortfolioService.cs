using System;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using loonloon.Grpc.Portfolio.Data.Interfaces;
using loonloon.Grpc.Portfolio.Service.Protos;

namespace loonloon.Grpc.Portfolio.Service.Services
{
    public class PortfolioService : Portfolios.PortfoliosBase
    {
        private readonly IPortfolioRepository _portfolioRepository;

        public PortfolioService(IPortfolioRepository portfolioRepository)
        {
            _portfolioRepository = portfolioRepository;
        }

        public override async Task<GetResponse> Get(GetRequest request, ServerCallContext context)
        {
            if (!Guid.TryParse(request.TraderId, out var traderId))
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "traderId must be a UUID"));
            }

            var portfolio = await _portfolioRepository.GetAsync(traderId, request.PortfolioId);

            return new GetResponse
            {
                Portfolio = Protos.Portfolio.FromRepositoryModel(portfolio)
            };
        }

        public override async Task<GetAllResponse> GetAll(GetAllRequest request, ServerCallContext context)
        {
            if (!Guid.TryParse(request.TraderId, out var traderId))
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "traderId must be a UUID"));
            }

            var portfolios = await _portfolioRepository.GetAllAsync(traderId);
            var response = new GetAllResponse();
            response.Portfolios.AddRange(portfolios.Select(Protos.Portfolio.FromRepositoryModel));
            return response;
        }
    }
}
