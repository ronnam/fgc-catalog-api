using Fgc.Catalog.Domain.Entities;

namespace Fgc.Catalog.Application.Interfaces
{
    /// <summary>
    /// Motor de busca full-text (Elasticsearch).
    /// Independente do repositório relacional - Fase 4, item 3.
    /// </summary>
    public interface IGameSearchService
    {
        /// <summary>Indexa ou atualiza um jogo no índice.</summary>
        Task IndexAsync(Game game, CancellationToken ct = default);

        /// <summary>Remove um jogo do índice.</summary>
        Task DeleteAsync(Guid gameId, CancellationToken ct = default);

        /// <summary>Busca com tolerância a erros de digitação (fuzzy)
        /// e ordenação por relevância (_score).
        /// </summary>
        Task<IReadOnlyList<GameSearchResult>> SearchAsync(
            string query, int page = 1, int pageSize = 10, CancellationToken ct = default
            );

        /// <summary>Recria o índice do zero e reindexa todos os jogos</summary>
        Task ReindexAllAsync(IEnumerable<Game> games, CancellationToken ct = default);

        /// <summary>Indica se o Elasticsearch está respondendo.</summary>
        Task<bool> IsAvailableAsync(CancellationToken ct = default);
    }

    public record GameSearchResult(
        Guid Id,
        string Title,
        string Category,
        decimal Price,
        double Score
    );
}
