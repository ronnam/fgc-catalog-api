using Fgc.MessageContracts.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Fgc.Catalog.Application.Consumers
{
    public class UserCreatedEventConsumer : IConsumer<UserCreatedEvent>
    {
        private readonly ILogger<UserCreatedEventConsumer> _logger;

        public UserCreatedEventConsumer(ILogger<UserCreatedEventConsumer> logger)
        {
            _logger = logger;
        }
        public async Task Consume(ConsumeContext<UserCreatedEvent> context)
        {
            var message = context.Message;

            _logger.LogInformation(
                "UserCreatedEvent received: UserId={UserId}, Username={Username}, Email={Email}",
                message.Id,
                message.Name,
                message.Email
                );

            // TODO: lógica de integração - criar catálogo do usuário, etc.

            await Task.CompletedTask;
        }
    }
}
