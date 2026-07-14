using Fgc.Catalog.Domain.Entities;
using Fgc.Catalog.Infrastructure.Persistence;
using Fgc.MessageContracts.Events;
using MassTransit;

namespace Fgc.Catalog.Api.Consumers
{
    public class UserCreatedEventConsumer(CatalogDbContext dbContext, ILogger<UserCreatedEventConsumer> logger) : IConsumer<UserCreatedEvent>
    {
        public async Task Consume(ConsumeContext<UserCreatedEvent> context)
        {
            var user = User.Create(
                context.Message.Name, 
                context.Message.Email, 
                context.Message.CreatedAt
            );

            await dbContext.Users.AddAsync(user);
            await dbContext.SaveChangesAsync(context.CancellationToken);

            logger.LogInformation("User created event consumed: {UserId}, {UserName}, {UserEmail}", user.Id, user.Name, user.Email);
        }
    }
}
