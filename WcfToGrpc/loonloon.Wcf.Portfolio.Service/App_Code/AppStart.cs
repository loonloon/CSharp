using loonloon.Wcf.Portfolio.Data;
using loonloon.Wcf.Portfolio.Data.Interfaces;
using Autofac;
using Autofac.Integration.Wcf;

namespace loonloon.Wcf.Portfolio.Service.App_Code
{
    public static class AppStart
    {
        public static void AppInitialize()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<PortfolioService>();
            builder.RegisterType<PortfolioRepository>().As<IPortfolioRepository>();
            AutofacHostFactory.Container = builder.Build();
        }
    }
}