using Fgc.Catalog.Application.DTOS.Games;
using Fgc.Catalog.Application.Interfaces;
using Fgc.Catalog.Domain.Entities;
using Fgc.Catalog.Domain.Exceptions;

namespace Fgc.Catalog.Application.Services
{
    public class GameService(IGameRepository repository)
    {
        public async Task<GameResponse> CreateAsync(GameRequest request)
        {
            if (await repository.ExistsByTitleAsync(request.Title))
                throw new CatalogDomainException("Game already exists.");

            var game = Game.Create(request.Title, request.Category, request.Price);

            await repository.AddAsync(game);

            return GameResponse.FromEntity(game);
        }

        public async Task<List<GameResponse>> GetAllAsync()
        {
            var games = await repository.GetAllAsync();
            return [.. games.Select(GameResponse.FromEntity).ToList()];
        }

        public async Task<GameResponse?> GetByIdAsync(Guid id)
        {
            var game = await repository.GetByIdAsync(id);
            return game is null?null : GameResponse.FromEntity(game);
        }

        public async Task<GameResponse>UpdateAsync(Guid id, GameRequest request)
        {
            var game = await repository.GetByIdAsync(id)
                ?? throw new NotFoundException("Game not found.");

            game.Update(request.Title, request.Category, request.Price);

            await repository.UpdateAsync(game);

            return GameResponse.FromEntity(game);
        }

        public async Task DeleteAsync(Guid id)
        {
            var game = await repository.GetByIdAsync(id)
                ?? throw new NotFoundException("Game not found.");
            await repository.DeleteAsync(game);
        }
    }
}

