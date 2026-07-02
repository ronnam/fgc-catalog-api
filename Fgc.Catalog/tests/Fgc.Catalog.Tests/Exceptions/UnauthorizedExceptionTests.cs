using Fgc.Catalog.Domain.Exceptions;

namespace Fgc.Catalog.Tests.Exceptions
{
    public class UnauthorizedExceptionTests
    {
        [Fact]
        public void Constructor_WithMessage_SetsMessage()
        {
            // Arrange
            var message = "The requested resource requires authentication.";

            // Act
            var exception = new UnauthorizedException(message);

            // Assert
            Assert.Equal(message, exception.Message);
        }
    }
}
