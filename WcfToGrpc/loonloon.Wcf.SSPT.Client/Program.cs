using System;
using System.ServiceModel;
using loonloon.Wcf.SSPT.Interface;

namespace loonloon.Wcf.SSPT.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            var binding = new NetTcpBinding(SecurityMode.None);
            var address = new EndpointAddress("net.tcp://localhost:12384/simplestockticker");
            var factory =
                new DuplexChannelFactory<ISimpleStockTickerService>(typeof(SimpleStockTickerCallback), binding,
                    address);
            var context = new InstanceContext(new SimpleStockTickerCallback());
            var server = factory.CreateChannel(context);

            server.Subscribe(new[] { "MSFT", "AAPL" });

            Console.WriteLine("Press E to exit...");

            var ch = ' ';

            while (ch != 'e')
            {
                ch = char.ToLowerInvariant((char)Console.Read());
            }
        }
    }
}
