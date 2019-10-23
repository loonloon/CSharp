using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;

namespace loonloon.Wcf.Portfolio.Service
{
    [ServiceContract]
    public interface IPortfolioService
    {
        [OperationContract]
        Task<Data.Models.Portfolio> Get(Guid traderId, int portfolioId);

        [OperationContract]
        Task<List<Data.Models.Portfolio>> GetAll(Guid traderId);
    }
}
