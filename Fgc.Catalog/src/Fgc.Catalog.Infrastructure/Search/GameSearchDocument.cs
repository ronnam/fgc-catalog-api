using Fgc.Catalog.Domain.Entities;

namespace Fgc.Catalog.Infrastructure.Search
{
    internal sealed class GameSearchDocument
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public double Price { get; set; }

        public static GameSearchDocument FromEntity(Game game) => new()
        {
            Id = game.Id,
            Title = game.Title,
            Category = game.Category,
            Price = (double)game.Price
        };
    }    
}
