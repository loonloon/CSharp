using System;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Net.Client;
using loonloon.Grpc.SSPT.Service.Protos;

namespace loonloon.Grpc.SSPT.Client
{
    class Program
    {
        static async Task Main(string[] args)
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            using var channel = GrpcChannel.ForAddress("http://localhost:5000");
            var client = new SimpleStockTicker.SimpleStockTickerClient(channel);

            var request = new SubscribeRequest();
            request.Symbols.AddRange(new[] { "MSFT", "AAPL" });
            using var stream = client.Subscribe(request);

            var tokenSource = new CancellationTokenSource();
            var task = DisplayAsync(stream.ResponseStream, tokenSource.Token);

            WaitForExitKey();

            tokenSource.Cancel();
            await task;
        }

        static async Task DisplayAsync(IAsyncStreamReader<StockTickerUpdate> stream, CancellationToken token)
        {
            try
            {
                await foreach (var update in stream.ReadAllAsync(token))
                {
                    Console.WriteLine($"{update.Symbol}: {update.Price}");
                }
            }
            catch (RpcException e)
            {
                if (e.StatusCode == StatusCode.Cancelled)
                {
                    return;
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Finished.");
            }
        }

        static void WaitForExitKey()
        {
            Console.WriteLine("Press E to exit...");

            var ch = ' ';

            while (ch != 'e')
            {
                ch = char.ToLowerInvariant(Console.ReadKey().KeyChar);
            }
        }
    }
}
