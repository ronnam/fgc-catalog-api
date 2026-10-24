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


    }
}
