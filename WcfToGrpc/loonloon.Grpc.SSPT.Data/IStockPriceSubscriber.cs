using System;

namespace loonloon.Grpc.SSPT.Data
{
    public interface IStockPriceSubscriber : IDisposable
    {
        event EventHandler<StockPriceUpdateEventArgs> Update;
    }
}
