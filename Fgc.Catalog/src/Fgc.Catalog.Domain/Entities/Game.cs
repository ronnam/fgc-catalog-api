using Fgc.Catalog.Domain.Exceptions;
using System.Diagnostics.CodeAnalysis;

namespace Fgc.Catalog.Domain.Entities
{
    public class Game
    {
        public Guid Id { get; private set; }
        public string Title { get; private set; } = null!;
        public string Category { get; private set; } = null!;
        public decimal Price { get; private set; }

        private Game() { }

        [SetsRequiredMembers]
        private Game(string title, string category, decimal price)
        {
            Id = Guid.NewGuid();
            Title = title;
            Category = category;
            Price = price;
        }

        public static Game Create(string title, string category, decimal price)
        {
            Validate(title, category, price);
            return new Game(title, category, price);
        }

        public void Update(string title, string category, decimal price)
        {
            Validate(title, category, price);
            Title = title;
            Category = category;
            Price = price;
        }

        private static void Validate(string title, string category, decimal price)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new CatalogDomainException("Title is required.");
            if (string.IsNullOrWhiteSpace(category))
                throw new CatalogDomainException("Category is required.");
            if (price <= 0)
                throw new CatalogDomainException("Price must be greater than zero.");
        }
    }
}