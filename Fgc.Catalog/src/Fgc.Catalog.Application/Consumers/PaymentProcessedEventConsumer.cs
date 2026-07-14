using Fgc.Catalog.Application.Interfaces;
using Fgc.Catalog.Domain.Entities;
using Fgc.MessageContracts.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Fgc.Catalog.Application.Consumers
{
    public class PaymentProcessedEventConsumer (
        IUserLibraryRepository  userLibraryRepository, 
        IGameRepository gameRepository,
        ILogger<PaymentProcessedEventConsumer> logger 
    ) : IConsumer<PaymentProcessedEvent>
    {
        public async Task Consume(ConsumeContext<PaymentProcessedEvent> context)
        {
            var message = context.Message;

            logger.LogInformation(
                "PaymentProcessedEvent received: OrderId={OrderId}, Status={Status}",
                message.OrderedId,
                message.Status);

            if (message.Status != "Approved")
            {
                logger.LogInformation("Payment not approved. Skipping library addition.");
                return;
            }

            // Verifica se o jogo existe
            var game = await gameRepository.GetByIdAsync(message.GameId);
            if (game is null)
            {
                logger.LogWarning("Game {GameId} not found. Skipping.", message.GameId);
                return;
            }

            // Verifica se já não foi adicionado antes (duplicidade)
            var alreadyExists = await userLibraryRepository.ExistsAsync(message.UserId, message.GameId);
            if (alreadyExists)
            {
                logger.LogInformation("Game {GameId} already in user {UserId} library. Skipping.", message.GameId, message.UserId);
                return;
            }

            // Adiciona o jogo na biblioteca do usuário
            var userLibrary = UserLibrary.Create(message.UserId, message.GameId, message.Price);
            await userLibraryRepository.AddAsync(userLibrary);

            logger.LogInformation(
                "Game {GameId} added to user {UserId} library successfully.",
                message.GameId, message.UserId);
        }
    }
}
