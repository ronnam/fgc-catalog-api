using Fgc.Catalog.Application.DTOS.Games;
using Fgc.Catalog.Application.Interfaces;
using Fgc.Catalog.Domain.Entities;
using Fgc.Catalog.Domain.Exceptions;

namespace Fgc.Catalog.Application.Services
{
    public class GameService
    {
        private readonly IGameRepository _repository;

        public GameService(IGameRepository repository)
        {
            _repository = repository;
        }

        public async Task<GameResponse> CreateAsync(GameRequest request)
        {
            if (await _repository.ExistsByTitleAsync(request.Title))
                throw new CatalogDomainException("Game already exists.");

            var game = Game.Create(request.Title, request.Category);

            await _repository.AddAsync(game);

            return GameResponse.FromEntity(game);
        }

        public async Task<List<GameResponse>> GetAllAsync()
        {
            var games = await _repository.GetAllAsync();
            return games.Select(GameResponse.FromEntity).ToList();
        }

        public async Task<GameResponse?> GetByIdAsync(Guid id)
        {
            var game = await _repository.GetByIdAsync(id);
            return game is null?null : GameResponse.FromEntity(game);
        }

        public async Task<GameResponse>UpdateAsync(Guid id, GameRequest request)
        {
            var game = await _repository.GetByIdAsync(id)
                ?? throw new NotFoundException("Game not found.");

            game.Update(request.Title, request.Category);

            await _repository.UpdateAsync(game);

            return GameResponse.FromEntity(game);
        }

        public async Task DeleteAsync(Guid id)
        {
            var game = await _repository.GetByIdAsync(id)
                ?? throw new NotFoundException("Game not found.");
            await _repository.DeleteAsync(game);
        }
    }
}

