using Fgc.Catalog.Domain.Entities;
using Fgc.Catalog.Domain.Exceptions;

namespace Fgc.Catalog.Tests.Entities
{
    public class UserTests
    {
        [Fact]
        public void Create_ValidData_ReturnsUser()
        {
            // Arrange
            var name = "John Doe";
            var email = "john.doe@example.com";
            var registeredAt = new DateTime(2026, 7, 8);

            // Act
            var user = User.Create(name, email, registeredAt);

            // Assert
            Assert.NotNull(user);
            Assert.NotEqual(Guid.Empty, user.Id);
            Assert.Equal(name, user.Name);
            Assert.Equal(email, user.Email);
            Assert.Equal(registeredAt, user.RegisteredAt);
        }

        [Fact]
        public void Create_EmptyName_ThrowsCatalogDomainException()
        {
            var ex = Assert.Throws<CatalogDomainException>(
                () => User.Create(string.Empty, "john.doe@example.com", DateTime.UtcNow));
            Assert.Equal("Name is required.", ex.Message);
        }

        [Fact]
        public void Create_NullName_ThrowsCatalogDomainException()
        {
            var ex = Assert.Throws<CatalogDomainException>(
                () => User.Create(null!, "john.doe@example.com", DateTime.UtcNow));
            Assert.Equal("Name is required.", ex.Message);
        }

        [Fact]
        public void Create_EmptyEmail_ThrowsCatalogDomainException()
        {
            var ex = Assert.Throws<CatalogDomainException>(
                () => User.Create("John Doe", string.Empty, DateTime.UtcNow));
            Assert.Equal("Email is required.", ex.Message);
        }

        [Fact]
        public void Create_NullEmail_ThrowsCatalogDomainException()
        {
            var ex = Assert.Throws<CatalogDomainException>(
                () => User.Create("John Doe", null!, DateTime.UtcNow));
            Assert.Equal("Email is required.", ex.Message);
        }
    }
}
