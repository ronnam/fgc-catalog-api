using Fgc.Catalog.Application.DTOS.Reviews;
using Fgc.Catalog.Application.Interfaces;
using Fgc.Catalog.Domain.Entities;
using Fgc.Catalog.Domain.Exceptions;
using MongoDB.Driver;

namespace Fgc.Catalog.Application.Services
{
    public class GameReviewService(IGameReviewRepository reviewRepository, IGameRepository gameRepository)
    {
        ///<summary>
        /// Cria uma avaliação. Regras: o jogo precisa existir e cada usuário
        /// só pode avaliar o mesmo jogo uma vez.
        ///</summary>
        
        public async Task<GameReviewResponse> CreateAsync(Guid gameId, Guid userId, GameReviewRequest request)
        {
            var game = await gameRepository.GetByIdAsync(gameId);
            if (game is null)
                throw new NotFoundException("Game not found");

            var existing = await reviewRepository.GetByGameAndUserAsync(gameId, userId);
            if (existing is not null)
                throw new CatalogDomainException("User has already reviewed this game");

            var review = new GameReview(gameId, userId, request.Rating, request.Comment);

            try
            {
                await reviewRepository.AddAsync(review);
            }
            catch (MongoWriteException ex) when (ex.WriteError.Category == ServerErrorCategory.DuplicateKey)
            {
                // Rede de segurança: duas requisições do mesmo usuário
                // passaram pela validação acima antes de qualquer inserção.
                // O índice único ux_game_user barra a segunda no banco.

                throw new CatalogDomainException("User has already reviewed this game");
            }

            return ToResponse(review);
        }

        public async Task<IReadOnlyList<GameReviewResponse>> GetByGameAsync(Guid gameId)
        {
            var reviews = await reviewRepository.GetByGameIdAsync(gameId);
            return reviews.Select(ToResponse).ToList();
        }

        private static GameReviewResponse ToResponse(GameReview r) => 
            new(
                r.Id,
                r.GameId,
                r.UserId,
                r.Rating,
                r.Comment,
                r.CreatedAt
            );
    }
}
