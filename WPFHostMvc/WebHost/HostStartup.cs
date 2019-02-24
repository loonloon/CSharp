using System;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using WPFHostMvc.Hub;

namespace WPFHostMvc.WebHost
{
    public class HostStartup
    {
        public IConfiguration Configuration { get; }

        public HostStartup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddCors(options => options.AddPolicy("CorsPolicy",
                builder =>
                {
                    builder.AllowAnyMethod().AllowAnyHeader()
                        .WithOrigins("http://127.0.0.1:9388")
                        .AllowCredentials();
                }));
            services.AddSignalR();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, System.IServiceProvider serviceProvider)
        {
            var currentPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? throw new InvalidOperationException(), "wwwroot");
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider($@"{currentPath}\js"),
                RequestPath = "/js"
            });
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider($@"{currentPath}\css"),
                RequestPath = "/css"
            });
            app.UseSignalR(routes => { routes.MapHub<ChatHub>("/chatHub"); });
            app.UseMvcWithDefaultRoute();
        }
    }
}
