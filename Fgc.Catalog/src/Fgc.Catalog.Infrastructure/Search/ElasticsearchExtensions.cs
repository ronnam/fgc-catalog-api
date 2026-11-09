using Elastic.Clients.Elasticsearch;
using Fgc.Catalog.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Fgc.Catalog.Infrastructure.Search
{
    public static class ElasticsearchExtensions
    {
        public const string IndexName = "games";

        public static IServiceCollection AddElasticsearchCatalog(
            this IServiceCollection services, IConfiguration configuration)
        {
            var uri = configuration["Elasticsearch:Uri"];

            if(string.IsNullOrWhiteSpace(uri))
            {
                // Fallback tolerante: a API sobe, mas a busca fica desabilitada.
                services.AddSingleton<IGameSearchService, NullGameSearchService>();
                return services;
            }

            var settings = new ElasticsearchClientSettings(new Uri(uri)).DefaultIndex(IndexName);

            services.AddSingleton(new ElasticsearchClient(settings));
            services.AddScoped<IGameSearchService, GameSearchService>();
            services.AddHostedService<GameSearchIndexInitializer>();

            return services;
        }
    }
    /// <summary>
    /// Usado quando Elasticsearch:Uri não está configurado.
    /// Mantém a API funcional — apenas sem busca.
    /// </summary>
    internal sealed class NullGameSearchService : IGameSearchService
    {
        public Task IndexAsync(Domain.Entities.Game game, CancellationToken ct = default) => Task.CompletedTask;
        public Task DeleteAsync(Guid gameId, CancellationToken ct = default) => Task.CompletedTask;
        public Task<IReadOnlyList<GameSearchResult>> SearchAsync(
            string query, int page = 1, int pageSize = 10, CancellationToken ct = default)
            => Task.FromResult<IReadOnlyList<GameSearchResult>>([]);
        public Task ReindexAllAsync(IEnumerable<Domain.Entities.Game> games, CancellationToken ct = default)
            => Task.CompletedTask;
        public Task<bool> IsAvailableAsync(CancellationToken ct = default) => Task.FromResult(false);
    }
}
