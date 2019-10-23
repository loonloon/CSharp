using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace loonloon.Grpc.Portfolio.Data.Interfaces
{
    public interface IPortfolioRepository
    {
        Task<Models.Portfolio> GetAsync(Guid traderId, int portfolioId);
        Task<List<Models.Portfolio>> GetAllAsync(Guid traderId);
    }
}
