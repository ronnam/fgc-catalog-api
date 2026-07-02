using Fgc.Catalog.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Fgc.Catalog.IntegrationTests.Infrastructure
{
    public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                services.RemoveAll(typeof(DbContextOptions<CatalogDbContext>));
                services.RemoveAll(typeof(DbContextOptions));

                services.AddScoped<DbContextOptions<CatalogDbContext>>(provider =>
                {
                    return new DbContextOptionsBuilder<CatalogDbContext>()
                        .UseInMemoryDatabase("InMemoryCatalogTestDb")
                        .Options;
                });
            });

        }
    }
}
