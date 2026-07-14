using Fgc.Catalog.Application.Interfaces;
using Fgc.Catalog.Application.Services;
using Fgc.Catalog.Domain.Entities;
using Fgc.Catalog.Domain.Exceptions;
using Fgc.MessageContracts.Events;
using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;

namespace Fgc.Catalog.Tests.Services
{
    public class PurchaseServiceTests
    {
        private readonly Mock<IGameRepository> _gameRepoMock;
        private readonly Mock<IPublishEndpoint> _publishEndpointMock;
        private readonly Mock<ILogger<PurchaseService>> _loggerMock;
        private readonly PurchaseService _service;

        public PurchaseServiceTests()
        {
            _gameRepoMock = new Mock<IGameRepository>();
            _publishEndpointMock = new Mock<IPublishEndpoint>();
            _loggerMock = new Mock<ILogger<PurchaseService>>();
            _service = new PurchaseService(
                _gameRepoMock.Object,
                _publishEndpointMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public async Task PurchaseAsync_ValidGame_PublishesOrderPlacedEvent()
        {
            var userId = Guid.NewGuid();
            var gameId = Guid.NewGuid();
            var game = Game.Create("Test Game", "Action", 59.90m);

            _gameRepoMock.Setup(r => r.GetByIdAsync(gameId))
                .ReturnsAsync(game);

            var result = await _service.PurchaseAsync(userId, gameId);

            Assert.NotNull(result);
            Assert.Equal(gameId, result.GameId);
            Assert.Equal(userId, result.UserId);

            _publishEndpointMock.Verify(e => e.Publish(
                It.Is<OrderPlacedEvent>(o =>
                    o.UserId == userId && o.GameId == gameId),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task PurchaseAsync_GameNotFound_ThrowsNotFoundException()
        {
            var userId = Guid.NewGuid();
            var gameId = Guid.NewGuid();

            _gameRepoMock.Setup(r => r.GetByIdAsync(gameId))
                .ReturnsAsync((Game?)null);

            var ex = await Assert.ThrowsAsync<NotFoundException>(() =>
                _service.PurchaseAsync(userId, gameId));

            Assert.Equal("Game not found.", ex.Message);
            _publishEndpointMock.Verify(e => e.Publish(
                It.IsAny<OrderPlacedEvent>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task PurchaseAsync_InvalidUserId_ThrowsCatalogDomainException()
        {
            var gameId = Guid.NewGuid();

            var ex = await Assert.ThrowsAsync<CatalogDomainException>(() =>
                _service.PurchaseAsync(Guid.Empty, gameId));

            Assert.Equal("UserId is required.", ex.Message);
            _publishEndpointMock.Verify(e => e.Publish(
                It.IsAny<OrderPlacedEvent>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task PurchaseAsync_InvalidGameId_ThrowsCatalogDomainException()
        {
            var userId = Guid.NewGuid();

            var ex = await Assert.ThrowsAsync<CatalogDomainException>(() =>
                _service.PurchaseAsync(userId, Guid.Empty));

            Assert.Equal("GameId is required.", ex.Message);
            _publishEndpointMock.Verify(e => e.Publish(
                It.IsAny<OrderPlacedEvent>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}