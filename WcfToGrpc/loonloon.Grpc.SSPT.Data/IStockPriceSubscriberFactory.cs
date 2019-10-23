namespace loonloon.Grpc.SSPT.Data
{
    public interface IStockPriceSubscriberFactory
    {
        IStockPriceSubscriber GetSubscriber(string[] symbols);
    }
}
