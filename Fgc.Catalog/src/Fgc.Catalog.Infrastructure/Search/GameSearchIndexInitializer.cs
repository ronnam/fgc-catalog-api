using Elastic.Clients.Elasticsearch;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Fgc.Catalog.Infrastructure.Search;

internal sealed class GameSearchIndexInitializer(
    ElasticsearchClient client,
    ILogger<GameSearchIndexInitializer> logger) : IHostedService
{
    private const int MaxAttempts = 5;
    private const int MaxBackoffSeconds = 5;

    public async Task StartAsync(CancellationToken ct)
    {
        for (var attempt = 1; attempt <= MaxAttempts; attempt++)
        {
            // Shutdown legítimo: não é falha, é o host fechando.
            if (ct.IsCancellationRequested)
            {
                logger.LogInformation(
                    "Inicializacao do indice cancelada pelo host (shutdown).");
                return;
            }

            try
            {
                var exists = await client.Indices.ExistsAsync(
                    ElasticsearchExtensions.IndexName, ct);

                if (!exists.IsValidResponse)
                {
                    logger.LogWarning(
                        "Tentativa {Attempt}/{Max}: consulta ao indice retornou resposta invalida. {Reason}",
                        attempt, MaxAttempts, exists.DebugInformation);
                }
                else if (exists.Exists)
                {
                    logger.LogInformation("Indice '{Index}' ja existe.",
                        ElasticsearchExtensions.IndexName);
                    return;
                }
                else
                {
                    var create = await client.Indices.CreateAsync(
                        ElasticsearchExtensions.IndexName, c => c
                            .Mappings(m => m
                                .Properties<GameSearchDocument>(p => p
                                    .Keyword(k => k.Id)
                                    .Text(t => t.Title, td => td.Analyzer("portuguese"))
                                    .Text(t => t.Category, td => td.Analyzer("portuguese"))
                                    .DoubleNumber(d => d.Price))), ct);

                    if (create.IsValidResponse)
                    {
                        logger.LogInformation("Indice '{Index}' criado.",
                            ElasticsearchExtensions.IndexName);
                        return;
                    }

                    logger.LogWarning(
                        "Tentativa {Attempt}/{Max} falhou ao criar indice: {Reason}",
                        attempt, MaxAttempts, create.DebugInformation);
                }
            }
            catch (OperationCanceledException) when (ct.IsCancellationRequested)
            {
                // Manuseado acima no próximo loop (ct.IsCancellationRequested).
                logger.LogInformation(
                    "Inicializacao do indice cancelada durante a tentativa {Attempt}.", attempt);
                return;
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex,
                    "Tentativa {Attempt}/{Max}: Elasticsearch indisponivel.", attempt, MaxAttempts);
            }

            if (attempt == MaxAttempts)
                break;

            // Backoff curto e com token de curta duracao:
            // se o host cancelar, encerramos limpo em vez de propagar excecao.
            var backoff = TimeSpan.FromSeconds(Math.Min(MaxBackoffSeconds, attempt));
            try
            {
                await Task.Delay(backoff, ct);
            }
            catch (OperationCanceledException)
            {
                logger.LogInformation(
                    "Inicializacao do indice interrompida por shutdown do host.");
                return;
            }
        }

        logger.LogError(
            "Elasticsearch nao respondeu apos {Max} tentativas. " +
            "A API continua operando; a busca fuzzy ficara degradada ate o servico ser restabelecido.",
            MaxAttempts);
    }

    public Task StopAsync(CancellationToken ct) => Task.CompletedTask;
}