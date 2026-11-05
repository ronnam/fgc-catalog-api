using Fgc.Catalog.Application.Interfaces;
using Fgc.Catalog.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Fgc.Catalog.IntegrationTests.Infrastructure
{
    public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                // --- Banco in-memory ---
                services.RemoveAll(typeof(DbContextOptions<CatalogDbContext>));
                services.RemoveAll(typeof(DbContextOptions));

                services.AddScoped<DbContextOptions<CatalogDbContext>>(provider =>
                {
                    return new DbContextOptionsBuilder<CatalogDbContext>()
                        .UseInMemoryDatabase("InMemoryCatalogTestDb")
                        .Options;
                });

                // --- Cache: mata o Redis e força MemoryDistributedCache ---
                services.RemoveAll<IDistributedCache>();
                services.RemoveAll<RedisCacheOptions>();

                services.AddSingleton<IDistributedCache>(_ =>
                    new MemoryDistributedCache(Options.Create(new MemoryDistributedCacheOptions())));

                // --- Mongo: repositório in-memory ---
                services.RemoveAll<IGameReviewRepository>();
                services.AddSingleton<IGameReviewRepository, InMemoryGameReviewRepository>();
            });
        }
    }
}