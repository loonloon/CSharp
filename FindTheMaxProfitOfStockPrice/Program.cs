using System;

namespace FindTheMaxProfitOfStockPrice
{
    class Program
    {
        static void Main(string[] args)
        {
            int[] stockPrices = { 30, 50, 3, 2, 1, 1, 10, 25, 2, 10 };
            var maxProfit = FindMaxProfit(stockPrices);
            Console.WriteLine($"Max Profit {maxProfit}");
        }

        public static int FindMaxProfit(int[] stockPricesYesterday)
        {
            // make sure we have at least 2 prices
            if (stockPricesYesterday.Length < 2)
            {
                throw new ArgumentException("Getting a profit requires at least 2 prices");
            }

            // we'll greedily update minPrice and maxProfit, so we initialize
            // them to the first price and the first possible profit
            var minPrice = stockPricesYesterday[0];
            var maxProfit = stockPricesYesterday[1] - stockPricesYesterday[0];

            // start at the second (index 1) time
            // we can't sell at the first time, since we must buy first,
            // and we can't buy and sell at the same time!
            // if we started at index 0, we'd try to buy *and* sell at time 0.
            // this would give a profit of 0, which is a problem if our
            // maxProfit is supposed to be *negative*--we'd return 0.
            for (var i = 1; i < stockPricesYesterday.Length; i++)
            {
                var currentPrice = stockPricesYesterday[i];

                // see what our profit would be if we bought at the
                // min price and sold at the current price
                var potentialProfit = currentPrice - minPrice;

                // update maxProfit if we can do better
                maxProfit = Math.Max(maxProfit, potentialProfit);

                // update minPrice so it's always
                // the lowest price we've seen so far
                minPrice = Math.Min(minPrice, currentPrice);
            }

            return maxProfit;
        }
    }
}
