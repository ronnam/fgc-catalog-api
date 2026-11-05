using Fgc.Catalog.Application.Interfaces;
using Fgc.Catalog.Domain.Entities;

namespace Fgc.Catalog.IntegrationTests.Infrastructure
{
    public class InMemoryGameReviewRepository : IGameReviewRepository
    {
        private readonly List<GameReview> _reviews = new();
        private readonly object _lock = new();

        public Task AddAsync(GameReview review, CancellationToken cancellationToken = default)
        {
            lock (_lock) { _reviews.Add(review); }
            return Task.CompletedTask;
        }

        public Task<IReadOnlyList<GameReview>> GetByGameIdAsync(Guid gameId, CancellationToken cancellationToken = default)
        {
            lock (_lock)
            {
                IReadOnlyList<GameReview> result = _reviews
                    .Where(r => r.GameId == gameId)
                    .OrderByDescending(r => r.CreatedAt)
                    .ToList();

                return Task.FromResult(result);
            }
        }

        public Task<GameReview?> GetByGameAndUserAsync(Guid gameId, Guid userId, CancellationToken cancellationToken = default)
        {
            lock (_lock)
            {
                var result = _reviews.FirstOrDefault(r => r.GameId == gameId && r.UserId == userId);
                return Task.FromResult(result);
            }
        }
    }
}