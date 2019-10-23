using System;

namespace loonloon.Wcf.SSPT.Data
{
    public class StockPriceUpdateEventArgs : EventArgs
    {
        public string Symbol { get; }
        public decimal Price { get; }

        public StockPriceUpdateEventArgs(string symbol, decimal price)
        {
            Symbol = symbol;
            Price = price;
        }
    }
}
