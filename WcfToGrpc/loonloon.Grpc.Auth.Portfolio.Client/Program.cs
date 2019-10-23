using System;
using System.Net.Http;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Net.Client;
using loonloon.Grpc.Auth.Portfolio.Service.Protos;

namespace loonloon.Grpc.Auth.Portfolio.Client
{
    class Program
    {
        private const string ServerAddress = "https://localhost:5001";

        static async Task Main(string[] args)
        {
            var token = await Authenticate("Alice");
            var channel = GrpcChannel.ForAddress(ServerAddress, new GrpcChannelOptions
            {
                Credentials = ChannelCredentials.Create(new SslCredentials(), CallCredentials.FromInterceptor((context, metadata) =>
                {
                    metadata.Add("Authorization", $"Bearer {token}");
                    return Task.CompletedTask;
                }))
            });
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
                if (e.StatusCode == StatusCode.PermissionDenied)
                {
                    Console.WriteLine("Permission denied.");
                }
            }
        }

        static async Task<string> Authenticate(string name)
        {
            using var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri($"{ServerAddress}/generateJwtToken?name={name}"),
                Method = HttpMethod.Get,
                Version = new Version(2, 0),
            };

            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }
    }
}
