using Fgc.Catalog.Domain.Exceptions;
using System.Diagnostics.CodeAnalysis;

namespace Fgc.Catalog.Domain.Entities
{
    public class Game
    {
        public Guid Id { get; private set; }
        public string Title { get; private set; } = null!;
        public  string Category { get; private set; } = null!;

        private Game() { }

        [SetsRequiredMembers]
        private Game(string title, string category)
        {
            Id = Guid.NewGuid();
            Title = title;
            Category = category;
        }

        public static Game Create(string title, string category)
        {
            Validate(title, category);

            return new Game(title, category);
        }

        public void Update(string title, string category)
        {
            Validate(title, category);
            Title = title;
            Category = category;
        }

        private static void Validate(string title, string category)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new CatalogDomainException("Title is required.");

            if (string.IsNullOrWhiteSpace(category))
                throw new CatalogDomainException("Category is required.");

        }
    }
}