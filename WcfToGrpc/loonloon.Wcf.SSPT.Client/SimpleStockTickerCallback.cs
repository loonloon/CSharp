using System;
using loonloon.Wcf.SSPT.Interface;

namespace loonloon.Wcf.SSPT.Client
{
    public class SimpleStockTickerCallback : ISimpleStockTickerCallback
    {
        public void Update(string symbol, decimal price)
        {
            Console.WriteLine($"{symbol}: {price}");
        }
    }
}
