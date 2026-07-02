using Fgc.Catalog.Domain.Entities;

namespace Fgc.Catalog.Application.DTOS.Games;

public class GameResponse
{
    public Guid Id { get; set; }
    public required string Title { get; set; }
    public required string Category { get; set; }

    public static GameResponse FromEntity(Game game)
    {
        return new GameResponse
        {
            Id = game.Id,
            Title = game.Title,
            Category = game.Category
        };
    }
}
