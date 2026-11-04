using Fgc.Catalog.Domain.Entities;

namespace Fgc.Catalog.Application.Interfaces
{
    public interface IGameReviewRepository
    {
        ///<summary>
        ///Persiste uma nova avaliação.
        ///</summary>
        Task AddAsync(GameReview review, CancellationToken cancellationToken = default);

        ///<summary>
        /// Lista as avaliações de um jogo, da mais recente para a mais antiga.
        /// </summary>
        Task<IReadOnlyList<GameReview>> GetByGameIdAsync(Guid gameId, CancellationToken cancellationToken = default);

        ///<summary>
        /// Busca a avalicação de um usuário específico para um jogo.
        /// Usado para validar a regra "1 avaliação por usuário por jogo".
        /// </summary>
        Task<GameReview?> GetByGameAndUserAsync(Guid gameId, Guid userId, CancellationToken cancellationToken = default);
    }
}
