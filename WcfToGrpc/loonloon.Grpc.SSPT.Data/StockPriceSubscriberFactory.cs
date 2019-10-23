namespace loonloon.Grpc.SSPT.Data
{
    public class StockPriceSubscriberFactory : IStockPriceSubscriberFactory
    {
        public IStockPriceSubscriber GetSubscriber(string[] symbols)
        {
            return new StockPriceSubscriber(symbols);
        }
    }
}
