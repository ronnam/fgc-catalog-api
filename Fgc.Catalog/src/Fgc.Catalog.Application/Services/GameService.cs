using System.Text.Json;
using Fgc.Catalog.Application.DTOS.Games;
using Fgc.Catalog.Application.Interfaces;
using Fgc.Catalog.Domain.Entities;
using Fgc.Catalog.Domain.Exceptions;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace Fgc.Catalog.Application.Services;

public class GameService(IGameRepository repository, IDistributedCache cache, ILogger<GameService> logger, IGameSearchService search)
{
    private static readonly TimeSpan CacheTtl = TimeSpan.FromSeconds(60);

    private static string CacheKey(Guid id) => $"game:{id}";

    public async Task<GameResponse> CreateAsync(GameRequest request)
    {
        if (await repository.ExistsByTitleAsync(request.Title))
            throw new CatalogDomainException("Game already exists.");

        var game = Game.Create(request.Title, request.Category, request.Price);
        await repository.AddAsync(game);
        await search.IndexAsync(game);

        return GameResponse.FromEntity(game);
    }

    public async Task<List<GameResponse>> GetAllAsync()
    {
        var games = await repository.GetAllAsync();
        return [.. games.Select(GameResponse.FromEntity)];
    }

    public async Task<GameResponse?> GetByIdAsync(Guid id)
    {
        var key = CacheKey(id);

        var cached = await cache.GetStringAsync(key);
        if (cached is not null)
        {
            logger.LogInformation("Cache HIT {Key}", key);
            return JsonSerializer.Deserialize<GameResponse>(cached);
        }

        logger.LogInformation("Cache MISS {Key}", key);

        var game = await repository.GetByIdAsync(id);
        if (game is null) return null;

        var response = GameResponse.FromEntity(game);
        await cache.SetStringAsync(key, JsonSerializer.Serialize(response),
            new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = CacheTtl });

        return response;
    }

    public async Task<GameResponse> UpdateAsync(Guid id, GameRequest request)
    {
        var game = await repository.GetByIdAsync(id)
            ?? throw new NotFoundException("Game not found.");

        game.Update(request.Title, request.Category, request.Price);
        await repository.UpdateAsync(game);

        await cache.RemoveAsync(CacheKey(id));

        await search.IndexAsync(game);

        return GameResponse.FromEntity(game);
    }

    public async Task DeleteAsync(Guid id)
    {
        var game = await repository.GetByIdAsync(id)
            ?? throw new NotFoundException("Game not found.");

        await repository.DeleteAsync(game);
        await cache.RemoveAsync(CacheKey(id));
        await search.DeleteAsync(id);
    }
}