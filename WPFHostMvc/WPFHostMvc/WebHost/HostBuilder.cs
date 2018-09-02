using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;

namespace WPFHostMvc.WebHost
{
    public static class HostBuilder
    {
        private static IWebHost _host;

        public static async Task Start()
        {
            if (_host == null)
            {
                var ip = System.Net.IPAddress.Parse("127.0.0.1");
                const int port = 9388;

                _host = new WebHostBuilder()
                    .UseKestrel(options =>
                    {
                        options.Listen(ip, port);
                    })
                    .UseStartup<HostStartup>()
                    .Build();

                await _host.RunAsync();
            }
        }
    }
}
