using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Fgc.Catalog.Domain.Entities
{
    public class GameReview
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; private set; }

        [BsonRepresentation(BsonType.String)]
        public Guid GameId { get; private set; }

        [BsonRepresentation(BsonType.String)]
        public Guid UserId { get; private set; }

        public int Rating { get; private set; }
        public string? Comment { get; private set; } = null!;
        public DateTime CreatedAt { get; private set; }

        private GameReview() { }

        public GameReview(Guid gameId, Guid userId, int rating, string? comment)
        {
            if (rating < 1 || rating > 5)
                throw new ArgumentOutOfRangeException(nameof(rating), "Rating must be between 1 and 5.");

            if (gameId == Guid.Empty)
                throw new ArgumentException("GameId is required.", nameof(gameId));

            if (userId == Guid.Empty)
                throw new ArgumentException("UserId is required.", nameof(userId));

            Id = Guid.NewGuid();
            GameId = gameId;
            UserId = userId;
            Rating = rating;
            Comment = string.IsNullOrWhiteSpace(comment) ? null : comment.Trim();
            CreatedAt = DateTime.UtcNow;
        }
    }
}