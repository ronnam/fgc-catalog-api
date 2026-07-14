using Fgc.Catalog.Application.DTOS.Purchases;
using Fgc.Catalog.Application.Interfaces;
using Fgc.Catalog.Domain.Exceptions;
using Fgc.MessageContracts.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Fgc.Catalog.Application.Services
{
    public class PurchaseService(
        IGameRepository gameRepository, 
        IPublishEndpoint publishEndpoint, 
        ILogger<PurchaseService> logger
    )
    {
        

        public async Task<PurchaseResponse> PurchaseAsync(Guid userId, Guid gameId)
        {
            if (userId == Guid.Empty)
                throw new CatalogDomainException("UserId is required.");

            if (gameId == Guid.Empty)
                throw new CatalogDomainException("GameId is required.");

            var game = await gameRepository.GetByIdAsync(gameId)
                ?? throw new NotFoundException("Game not found.");

            var orderId = Guid.NewGuid();

            var orderPlacedEvent = new OrderPlacedEvent(
                OrderId: orderId,
                UserId: userId,
                GameId: gameId,
                Price: game.Price);

            await publishEndpoint.Publish(orderPlacedEvent);

            logger.LogInformation(
                "OrderPlacedEvent published: {OrderId} for User {UserId} - Game {GameId} at {Price}",
                orderId, userId, gameId, game.Price);

            return new PurchaseResponse
            {
                OrderId = orderId,
                UserId = userId,
                GameId = gameId,
                Price = game.Price
            };
        }
    }
}