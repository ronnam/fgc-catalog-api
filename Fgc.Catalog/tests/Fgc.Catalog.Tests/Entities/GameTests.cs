using Fgc.Catalog.Domain.Entities;
using Fgc.Catalog.Domain.Exceptions;

namespace Fgc.Catalog.Tests.Entities
{
    public class GameTests
    {
        [Fact]
        public void Create_ValidData_ReturnsGame()
        {
            var game = Game.Create("Test Game", "Action", 59.99m);

            Assert.NotNull(game);
            Assert.NotEqual(Guid.Empty, game.Id);
            Assert.Equal("Test Game", game.Title);
            Assert.Equal("Action", game.Category);
            Assert.Equal(59.99m, game.Price);
        }

        [Fact]
        public void Create_EmptyTitle_ThrowsCatalogDomainException()
        {
            var ex = Assert.Throws<CatalogDomainException>(
                () => Game.Create(string.Empty, "Action", 59.99m));
            Assert.Equal("Title is required.", ex.Message);
        }

        [Fact]
        public void Create_NullTitle_ThrowsCatalogDomainException()
        {
            var ex = Assert.Throws<CatalogDomainException>(
                () => Game.Create(null!, "Action", 59.99m));
            Assert.Equal("Title is required.", ex.Message);
        }

        [Fact]
        public void Create_EmptyCategory_ThrowsCatalogDomainException()
        {
            var ex = Assert.Throws<CatalogDomainException>(
                () => Game.Create("Test Game", string.Empty, 59.99m));
            Assert.Equal("Category is required.", ex.Message);
        }

        [Fact]
        public void Create_NullCategory_ThrowsCatalogDomainException()
        {
            var ex = Assert.Throws<CatalogDomainException>(
                () => Game.Create("Test Game", null!, 59.99m));
            Assert.Equal("Category is required.", ex.Message);
        }

        [Fact]
        public void Create_ZeroPrice_ThrowsCatalogDomainException()
        {
            var ex = Assert.Throws<CatalogDomainException>(
                () => Game.Create("Test Game", "Action", 0));
            Assert.Equal("Price must be greater than zero.", ex.Message);
        }

        [Fact]
        public void Create_NegativePrice_ThrowsCatalogDomainException()
        {
            var ex = Assert.Throws<CatalogDomainException>(
                () => Game.Create("Test Game", "Action", -10));
            Assert.Equal("Price must be greater than zero.", ex.Message);
        }

        [Fact]
        public void Update_ValidData_UpdatesGame()
        {
            var game = Game.Create("Old Title", "Old Category", 10m);

            game.Update("New Title", "New Category", 99.99m);

            Assert.Equal("New Title", game.Title);
            Assert.Equal("New Category", game.Category);
            Assert.Equal(99.99m, game.Price);
        }
    }
}