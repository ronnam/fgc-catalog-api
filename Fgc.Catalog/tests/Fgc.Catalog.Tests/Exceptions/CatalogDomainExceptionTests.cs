using Fgc.Catalog.Domain.Exceptions;

namespace Fgc.Catalog.Tests.Exceptions
{
    public class CatalogDomainExceptionTests
    {
        [Fact]
        public void Constructor_WithMessage_SetsMessage()
        {
            // Arrange
            var message = "This is a domain exception.";

            // Act
            var exception = new CatalogDomainException(message);

            // Assert
            Assert.Equal(message, exception.Message);
        }
    }
}
