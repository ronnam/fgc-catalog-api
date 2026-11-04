namespace Fgc.Catalog.Application.DTOS.Reviews
{
    public record GameReviewResponse(
        Guid id,
        Guid GameId,
        Guid UserId,
        int Rating,
        string? Comment,
        DateTime CreatedAt

    );
}
