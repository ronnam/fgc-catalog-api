using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Fgc.Catalog.Application.Interfaces;
using Fgc.Catalog.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Fgc.Catalog.Infrastructure.Search;

internal sealed class GameSearchService(ElasticsearchClient client, ILogger<GameSearchService> logger) : IGameSearchService
{
    private static readonly string[] SearchFields = ["title^3", "category^1"];

    public async Task IndexAsync(Game game, CancellationToken ct = default)
    {
        var doc = new GameSearchDocument
        {
            Id = game.Id,
            Title = game.Title,
            Category = game.Category,
            Price = (double)game.Price
        };

        var response = await client.IndexAsync(doc, i => i
            .Index(ElasticsearchExtensions.IndexName)
            .Id(game.Id.ToString()), ct);

        if (!response.IsValidResponse)
            throw new InvalidOperationException(
                $"Falha ao indexar jogo {game.Id}: {response.DebugInformation}");

        logger.LogInformation("Jogo {Id} indexado.", game.Id);
    }

    public async Task<bool> IsAvailableAsync(CancellationToken ct = default)
    {
        try
        {
            var ping = await client.PingAsync(ct);
            return ping.IsValidResponse;
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Elasticsearch nao respondeu ao ping.");
            return false;
        }
    }

    public async Task DeleteAsync(Guid gameId, CancellationToken ct = default)
    {
        var response = await client.DeleteAsync<GameSearchDocument>(
            gameId.ToString(), d => d.Index(ElasticsearchExtensions.IndexName), ct);

        if (!response.IsValidResponse && response.Result != Result.NotFound)
            logger.LogWarning("Falha ao remover jogo {Id} do indice: {Info}",
                gameId, response.DebugInformation);
    }

    public async Task<IReadOnlyList<GameSearchResult>> SearchAsync(
        string query, int page = 1, int pageSize = 10, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(query))
            return [];

        try
        {
            var response = await client.SearchAsync<GameSearchDocument>(s => s
                .Indices(ElasticsearchExtensions.IndexName)
                .From(Math.Max(0, (page - 1) * pageSize))
                .Size(pageSize)
                .Query(q => q
                    .MultiMatch(mm => mm
                        .Query(query)
                        .Fields(SearchFields)
                        .Type(TextQueryType.BestFields)
                        .Fuzziness(new Fuzziness("AUTO"))
                        .MinimumShouldMatch("70%")))
                .Sort(sort => sort.Score(new ScoreSort { Order = SortOrder.Desc })), ct);

            return [.. response.Hits
                .Where(h => h.Source is not null)
                .Select(h => new GameSearchResult(
                    h.Source!.Id,
                    h.Source.Title,
                    h.Source.Category,
                    (decimal)h.Source.Price,
                    h.Score ?? 0))];
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao buscar '{Query}' no Elasticsearch.", query);
            return [];
        }
    }

    public async Task ReindexAllAsync(IEnumerable<Game> games, CancellationToken ct = default)
    {
        foreach (var game in games)
            await IndexAsync(game, ct);
    }
}