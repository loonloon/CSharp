using System;
using System.ServiceModel;
using loonloon.Wcf.SSPT.Interface;

namespace loonloon.Wcf.SSPT.Service
{
    class Program
    {
        static void Main(string[] args)
        {
            var host = new ServiceHost(typeof(SimpleStockTickerService));
            var binding = new NetTcpBinding(SecurityMode.None);
            host.AddServiceEndpoint(typeof(ISimpleStockTickerService), binding, "net.tcp://localhost:12384/simplestockticker");
            host.Open();

            Console.WriteLine("Press E to exit...");

            var ch = ' ';

            while (ch != 'e')
            {
                ch = char.ToLowerInvariant((char)Console.Read());
            }

            host.Close();
        }
    }
}
