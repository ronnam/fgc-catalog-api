using Fgc.Catalog.Domain.Exceptions;
using System.Diagnostics.CodeAnalysis;

namespace Fgc.Catalog.Domain.Entities
{
    public class UserLibrary
    {
        public Guid Id { get; private set; }
        public Guid UserId { get; private set; }
        public Guid GameId { get; private set; }
        public decimal Price { get; private set; }
        public DateTime PurchasedAt { get; private set; }

        private UserLibrary() { }

        [SetsRequiredMembers]
        private UserLibrary(Guid userId, Guid gameId, decimal price)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            GameId = gameId;
            Price = price;
            PurchasedAt = DateTime.UtcNow;
        }

        public static UserLibrary Create(Guid userId, Guid gameId, decimal price)
        {
            Validate(userId, gameId, price);
            return new UserLibrary(userId, gameId, price);
        }

        private static void Validate(Guid userId, Guid gameId, decimal price)
        {
            if (userId == Guid.Empty)
                throw new CatalogDomainException("UserId is required.");
            if (gameId == Guid.Empty)
                throw new CatalogDomainException("GameId is required.");
            if (price <= 0)
                throw new CatalogDomainException("Price must be greater than zero.");
        }
    }
}