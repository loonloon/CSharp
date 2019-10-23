using System.ServiceModel;

namespace loonloon.Wcf.SSPT.Interface
{
    [ServiceContract]
    public interface ISimpleStockTickerCallback
    {
        [OperationContract(IsOneWay = true)]
        void Update(string symbol, decimal price);
    }
}
