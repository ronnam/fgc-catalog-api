using Fgc.Catalog.Domain.Exceptions;

namespace Fgc.Catalog.Tests.Exceptions
{
    public class ConflictExceptionTests
    {
        [Fact]
        public void Constructor_WithMessage_SetsMessage()
        {
            // Arrange
            var message = "A conflict occurred while processing the request.";

            // Act
            var exception = new ConflictException(message);

            // Assert
            Assert.Equal(message, exception.Message);
        }
}
}
