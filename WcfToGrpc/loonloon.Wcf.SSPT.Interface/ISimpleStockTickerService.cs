using System.ServiceModel;

namespace loonloon.Wcf.SSPT.Interface
{
    [ServiceContract(SessionMode = SessionMode.Required, CallbackContract = typeof(ISimpleStockTickerCallback))]
    public interface ISimpleStockTickerService
    {
        [OperationContract(IsOneWay = true)]
        void Subscribe(string[] symbols);
    }
}
