using System.Security.Claims;
using loonloon.Grpc.Auth.Portfolio.Data;
using loonloon.Grpc.Auth.Portfolio.Data.Interfaces;
using loonloon.Grpc.Auth.Portfolio.Service.Interfaces;
using loonloon.Grpc.Auth.Portfolio.Service.Services;
using loonloon.Grpc.Auth.Portfolio.Service.Utilities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

namespace loonloon.Grpc.Auth.Portfolio.Service
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IPortfolioRepository, PortfolioRepository>();
            services.AddSingleton<IUserLookup, UserLookup>();
            services.AddGrpc();
            services.AddAuthorization(options =>
            {
                options.AddPolicy(JwtBearerDefaults.AuthenticationScheme, policy =>
                {
                    policy.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme);
                    policy.RequireClaim(ClaimTypes.Name);
                });
            });
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options => 
            { 
                options.TokenValidationParameters = new TokenValidationParameters 
                {
                    ValidateAudience = false, 
                    ValidateIssuer = false, 
                    ValidateActor = false, 
                    ValidateLifetime = true, 
                    IssuerSigningKey = JwtHelper.SecurityKey
                };
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<PortfolioService>();
                endpoints.MapGet("/generateJwtToken", context =>
                    context.Response.WriteAsync(JwtHelper.GenerateJwtToken(context.Request.Query["name"])));
            });
        }
    }
}
