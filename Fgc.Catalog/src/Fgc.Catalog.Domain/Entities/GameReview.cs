namespace Fgc.Catalog.Domain.Entities
{
    public class GameReview
    {
        public Guid Id { get; private set; }
        public Guid GameId { get; private set; }
        public Guid UserId { get; private set; }
        public int Rating { get; private set; }
        public string Comment { get; private set; } = null!;
        public DateTime CreatedAt { get; private set; }
    }
}
