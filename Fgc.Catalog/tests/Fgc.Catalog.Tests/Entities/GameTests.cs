using Fgc.Catalog.Domain.Entities;
using Fgc.Catalog.Domain.Exceptions;

namespace Fgc.Catalog.Tests.Entities
{
    public class GameTests
    {
        [Fact]
        public void Create_ValidTitleAndCategory_ReturnsGame()
        {
            // Arrange
            var title = "Super Mario";
            var category = "Plataforma";

            // Act
            var game = Game.Create(title, category);

            // Assert
            Assert.NotNull(game);
            Assert.NotEqual(Guid.Empty, game.Id);
            Assert.Equal(title, game.Title);
            Assert.Equal(category, game.Category);
        }

        [Fact]
        public void Create_EmptyTitle_ThrowsCatalogDomainException()
        {
            var ex = Assert.Throws<CatalogDomainException>(() => Game.Create(string.Empty, "Plataforma"));
            Assert.Equal("Title is required.", ex.Message);
        }

        [Fact]
        public void Create_NullTitle_ThrowsCatalogDomainException()
        {
            var ex = Assert.Throws<CatalogDomainException>(() => Game.Create(null!, "Plataforma"));
            Assert.Equal("Title is required.", ex.Message);
        }

        [Fact]
        public void Create_EmptyCategory_ThrowsCatalogDomainException()
        {
            var ex = Assert.Throws<CatalogDomainException>(() => Game.Create("Super Mario", string.Empty));
            Assert.Equal("Category is required.", ex.Message);
        }
    }
}
