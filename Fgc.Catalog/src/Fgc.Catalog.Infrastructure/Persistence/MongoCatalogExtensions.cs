using Fgc.Catalog.Application.Interfaces;
using Fgc.Catalog.Infrastructure.Persistence.Mappings;
using Fgc.Catalog.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace Fgc.Catalog.Infrastructure.Persistence
{
    public static class MongoCatalogExtensions
    {
        public static IServiceCollection AddMongoCatalog(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("MongoDb")
                ?? throw new InvalidOperationException("Connection string 'MongoDb' não configurada.");
            var databaseName = configuration["MongoDb:DatabaseName"]
                ?? throw new InvalidOperationException("MongoDb:DatabaseName não configurado.");

            GameReviewClassMap.Register();

            services.AddSingleton<IMongoClient>(_ => new MongoClient(connectionString));
            services.AddScoped<IMongoDatabase>(sp =>
                sp.GetRequiredService<IMongoClient>().GetDatabase(databaseName));
            services.AddScoped<IGameReviewRepository, GameReviewMongoRepository>();

            return services;
        }
    }
}
