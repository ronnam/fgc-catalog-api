using Elastic.Clients.Elasticsearch;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Fgc.Catalog.Infrastructure.Search;

internal sealed class GameSearchIndexInitializer : IHostedService
{
    private readonly ElasticsearchClient _client;
    private readonly ILogger<GameSearchIndexInitializer> _logger;

    public GameSearchIndexInitializer(
        ElasticsearchClient client,
        ILogger<GameSearchIndexInitializer> logger)
    {
        _client = client;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken ct)
    {
        for (var attempt = 1; attempt <= 5; attempt++)
        {
            try
            {
                var exists = await _client.Indices.ExistsAsync(
                    ElasticsearchExtensions.IndexName, ct);

                if (exists.Exists)
                {
                    _logger.LogInformation("Indice '{Index}' ja existe.",
                        ElasticsearchExtensions.IndexName);
                    return;
                }

                var create = await _client.Indices.CreateAsync(
                    ElasticsearchExtensions.IndexName, c => c
                        .Mappings(m => m
                            .Properties<GameSearchDocument>(p => p
                                .Keyword(k => k.Id)
                                .Text(t => t.Title, td => td.Analyzer("portuguese"))
                                .Text(t => t.Category, td => td.Analyzer("portuguese"))
                                .DoubleNumber(d => d.Price))), ct);

                if (create.IsValidResponse)
                {
                    _logger.LogInformation("Indice '{Index}' criado.",
                        ElasticsearchExtensions.IndexName);
                    return;
                }

                _logger.LogWarning("Tentativa {Attempt}/5 falhou ao criar indice: {Reason}",
                    attempt, create.DebugInformation);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Tentativa {Attempt}/5 - Elasticsearch indisponivel.", attempt);
            }

            await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, attempt)), ct);
        }

        _logger.LogError(
            "Elasticsearch nao respondeu apos 5 tentativas. " +
            "A API continua operando com o banco relacional; a busca ficara indisponivel.");
    }

    public Task StopAsync(CancellationToken ct) => Task.CompletedTask;
}