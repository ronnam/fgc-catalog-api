using Fgc.Catalog.Application.Consumers;
using Fgc.Catalog.Application.Interfaces;
using Fgc.Catalog.Domain.Entities;
using Fgc.MessageContracts.Events;
using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;

namespace Fgc.Catalog.Tests.Consumers
{
    public class PaymentProcessedEventConsumerTests
    {
        private readonly Mock<IUserLibraryRepository> _userLibraryRepoMock;
        private readonly Mock<IGameRepository> _gameRepoMock;
        private readonly Mock<ILogger<PaymentProcessedEventConsumer>> _loggerMock;
        private readonly PaymentProcessedEventConsumer _consumer;

        public PaymentProcessedEventConsumerTests()
        {
            _userLibraryRepoMock = new Mock<IUserLibraryRepository>();
            _gameRepoMock = new Mock<IGameRepository>();
            _loggerMock = new Mock<ILogger<PaymentProcessedEventConsumer>>();
            _consumer = new PaymentProcessedEventConsumer(
                _userLibraryRepoMock.Object,
                _gameRepoMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public async Task Consume_ApprovedPaymentAndGameExists_AddsToLibrary()
        {
            var gameId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var message = new PaymentProcessedEvent
            (
                OrderedId: Guid.NewGuid(),
                UserId: userId,
                GameId: gameId,
                Price: 100.50m,
                Status: "Approved",
                ProcessedAt: DateTime.UtcNow
            );
            var context = Mock.Of<ConsumeContext<PaymentProcessedEvent>>(c => c.Message == message);

            _gameRepoMock.Setup(r => r.GetByIdAsync(gameId))
                .ReturnsAsync(Game.Create("Test Game", "Action"));
            _userLibraryRepoMock.Setup(r => r.ExistsAsync(userId, gameId))
                .ReturnsAsync(false);

            try
            {
                await _consumer.Consume(context);
            }
            catch (Exception ex)
            {
                Assert.Fail($"Consumer lançou exceção: {ex.GetType().Name} - {ex.Message}");
                // Diagnóstico: quantas vezes os mocks foram chamados?
                var gameRepoInvocations = _gameRepoMock.Invocations.Count;
                var userLibInvocations = _userLibraryRepoMock.Invocations.Count;
                Assert.True(gameRepoInvocations > 0,
                    $"gameRepo foi chamado {gameRepoInvocations} vez(es). " +
                    $"userLibraryRepo foi chamado {userLibInvocations} vez(es).");
            }

            _userLibraryRepoMock.Verify(r => r.AddAsync(It.Is<UserLibrary>(u =>
                u.UserId == userId &&
                u.GameId == gameId &&
                u.Price == 100.50m)), Times.Once);
        }

        [Fact]
        public async Task Consume_PaymentNotApproved_SkipsLibraryAddition()
        {
            var gameId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var message = new PaymentProcessedEvent
            (
                Guid.NewGuid(),
                gameId,
                userId,
                100.50m,
                "Pending",
                DateTime.UtcNow
            );
            var context = Mock.Of<ConsumeContext<PaymentProcessedEvent>>(c => c.Message == message);

            await _consumer.Consume(context);

            _userLibraryRepoMock.Verify(r => r.AddAsync(It.IsAny<UserLibrary>()), Times.Never);
        }

        [Fact]
        public async Task Consume_GameNotFound_SkipsLibraryAddition()
        {
            var userId = Guid.NewGuid();
            var gameId = Guid.NewGuid();
            var message = new PaymentProcessedEvent
            (
                Guid.NewGuid(),
                gameId,
                userId,
                100.50m,
                "Approved",
                DateTime.UtcNow
            );
            var context = Mock.Of<ConsumeContext<PaymentProcessedEvent>>(c => c.Message == message);

            _gameRepoMock.Setup(r => r.GetByIdAsync(gameId))
                .ReturnsAsync((Game?)null);

            await _consumer.Consume(context);

            _userLibraryRepoMock.Verify(r => r.AddAsync(It.IsAny<UserLibrary>()), Times.Never);
        }

        [Fact]
        public async Task Consume_GameAlreadyInLibrary_SkipsDuplication()
        {
            var gameId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var message = new PaymentProcessedEvent
            (
                Guid.NewGuid(),
                gameId,
                userId,
                50,
                "Approved",
                DateTime.UtcNow
            );
            var context = Mock.Of<ConsumeContext<PaymentProcessedEvent>>(c => c.Message == message);

            _gameRepoMock.Setup(r => r.GetByIdAsync(gameId))
                .ReturnsAsync(Game.Create("Test Game", "Action"));
            _userLibraryRepoMock.Setup(r => r.ExistsAsync(userId, gameId))
                .ReturnsAsync(true);

            await _consumer.Consume(context);

            _userLibraryRepoMock.Verify(r => r.AddAsync(It.IsAny<UserLibrary>()), Times.Never);
        }
    }
}