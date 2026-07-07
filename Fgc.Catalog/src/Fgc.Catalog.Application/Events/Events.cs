namespace Fgc.Catalog.Application.Events
{
    public record UserCreatedEvent(Guid UserId, string Name, string Email);
}