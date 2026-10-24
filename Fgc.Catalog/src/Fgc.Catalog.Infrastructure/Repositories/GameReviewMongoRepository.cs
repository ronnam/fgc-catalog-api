using Fgc.Catalog.Application.Interfaces;
using Fgc.Catalog.Domain.Entities;
using Fgc.Catalog.Infrastructure.Persistence;
using MongoDB.Driver;

namespace Fgc.Catalog.Infrastructure.Repositories
{
    public class GameReviewMongoRepository(IMongoDatabase database) : IGameReviewRepository
    {
        private readonly IMongoCollection<GameReview> _collection = database.GetCollection<GameReview>(MongoCollectionNames.GameReviews);

        public async Task AddAsync(GameReview review, CancellationToken cancellationToken = default)
        {
            await _collection.InsertOneAsync(review, cancellationToken: cancellationToken);
        }

        public async Task<IReadOnlyList<GameReview>> GetByGameIdAsync(Guid gameId, CancellationToken cancellationToken = default)
        {
            var filter = Builders<GameReview>.Filter.Eq(r => r.GameId, gameId);
            var sort = Builders<GameReview>.Sort.Descending(r => r.CreatedAt);

            var reviews = await _collection
                .Find(filter)
                .Sort(sort)
                .ToListAsync(cancellationToken);

            return reviews.AsReadOnly();
        }
    }
}
