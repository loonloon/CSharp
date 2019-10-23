using System;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Net.Client;
using loonloon.Grpc.Portfolio.Service.Protos;

namespace loonloon.Grpc.Portfolio.Client
{
    class Program
    {
        static async Task Main(string[] args)
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            const string serverAddress = "http://localhost:5000";
            var channel = GrpcChannel.ForAddress(serverAddress);
            var portfolios = new Portfolios.PortfoliosClient(channel);

            try
            {
                var request = new GetRequest
                {
                    TraderId = "68CB16F7-42BD-4330-A191-FA5904D2E5A0",
                    PortfolioId = 42
                };

                var response = await portfolios.GetAsync(request);
                Console.WriteLine($"Portfolio contains {response.Portfolio.Items.Count} items.");
            }
            catch (RpcException e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
