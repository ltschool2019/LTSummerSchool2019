using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LTRegistrator.BLL.Services;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LTRegistratorApi.Tests.Integrations
{
    public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        protected override IWebHostBuilder CreateWebHostBuilder()
        {
            return WebHost.CreateDefaultBuilder().UseStartup<TStartup>();
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var serviceProvider = new ServiceCollection()
                    .AddEntityFrameworkInMemoryDatabase()
                    .BuildServiceProvider();

                services.AddDbContext<DbContext, LTRegistratorDbContext>(options =>
                {
                    string connectionString = "Server=(localdb)\\mssqllocaldb;Database=productsdb;Trusted_Connection=True;";
                    options.UseInMemoryDatabase(connectionString);
                    options.UseInternalServiceProvider(serviceProvider);
                });
            });
        }
    }
}
