using loonloon.Wcf.Portfolio.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace loonloon.Wcf.Portfolio.Service
{
  
    public class PortfolioService : IPortfolioService
    {
        private readonly IPortfolioRepository _repository;

        public PortfolioService()
        {

        }

        public PortfolioService(IPortfolioRepository repository)
        {
            _repository = repository;
        }

        public async Task<Data.Models.Portfolio> Get(Guid traderId, int portfolioId)
        {
            return await _repository.GetAsync(traderId, portfolioId);
        }

        public async Task<List<Data.Models.Portfolio>> GetAll(Guid traderId)
        {
            return await _repository.GetAllAsync(traderId);
        }
    }
}
