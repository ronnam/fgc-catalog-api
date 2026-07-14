using Fgc.Catalog.Application.Events;
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
        public Task Consume(ConsumeContext<UserCreatedEvent> context)
        {
            var evento = context.Message;

            _logger.LogInformation(
                "UserCreatedEvent received: UserId={UserId}, Name={Name}, Email={Email}", 
                evento.UserId,
                evento.Name,
                evento.Email
            );

            return Task.CompletedTask;
        }
    }
}
