using Fgc.Catalog.Domain.Exceptions;

namespace Fgc.Catalog.Tests.Exceptions
{
    public class NotFoundExceptionTests
    {
        [Fact]
        public void Constructor_WithMessage_SetsMessage()
        {
            // Arrange
            var message = "The requested resource was not found.";

            // Act
            var exception = new NotFoundException(message);

            // Assert
            Assert.Equal(message, exception.Message);
        }
    }
}
