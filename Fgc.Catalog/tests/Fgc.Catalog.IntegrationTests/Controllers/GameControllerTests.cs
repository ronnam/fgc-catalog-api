using Fgc.Catalog.Application.DTOS.Games;
using Fgc.Catalog.Infrastructure.Persistence;
using Fgc.Catalog.IntegrationTests.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
namespace Fgc.Catalog.IntegrationTests.Controllers
{
    public class GameControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory<Program> _factory;

        public GameControllerTests(CustomWebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
            _factory = factory;

            ResetDatabase();
        }

        private void ResetDatabase()
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
        }

        private static StringContent CreateJsonContent(object obj)
        {
            var json = System.Text.Json.JsonSerializer.Serialize(obj);
            return new StringContent(json, Encoding.UTF8, "application/json");
        }

        private static string GenerateAdminToken()
        {
            var key = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes("FgcUsers-Auth-JWT-Key-2026-Strong-And-Secure")
                );

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Role, "Admin"),
                new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
            };

            var token = new JwtSecurityToken(
                issuer: "Fgc.UsersAPI",
                audience: "Fgc.CatalogAPI",
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [Fact]
        public async Task CreateGame_WithValidDataAndAdminToken_Returns201()
        {             
            // Arrange
            var request = new GameRequest { Title = "Super Mario", Category = "Plataforma" };
            var token = GenerateAdminToken();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await _client.PostAsJsonAsync("/games", request);

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var gameResponse = await response.Content.ReadFromJsonAsync<GameResponse>();
            Assert.NotNull(gameResponse);
            Assert.Equal(request.Title, gameResponse.Title);
        }

        [Fact]
        public async Task CreateGame_WithEmptyTitle_Retuns400()
        {
            // Arrange
            var token = GenerateAdminToken();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var request = new GameRequest { Title = "", Category = "Plataforma" };

            // Act
            var response = await _client.PostAsJsonAsync("/games", request);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task CreateGame_WithoutToken_Returns401()
        {
            // Arrange - sem token
            var request = new GameRequest { Title = "Super Mario", Category = "Plataforma" };

            // Act
            var response = await _client.PostAsJsonAsync("/games", request);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task CreateGame_WithNonAdminToken_Returns403()
        {
            // Arrange - Token de usuário não admin
            var key = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes("FgcUsers-Auth-JWT-Key-2026-Strong-And-Secure")
                );
            var credencials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim(ClaimTypes.Role, "User"), // Não é Admin
            };
            var token = new JwtSecurityToken(
                issuer: "Fgc.UsersAPI",
                audience: "Fgc.CatalogAPI",
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credencials
            );
            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenString);
            var request = new GameRequest { Title = "Super Mario", Category = "Plataforma" };

            // Act
            var response = await _client.PostAsJsonAsync("/games", request);

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task GetAll_WhenGamesExist_Returns200WhithList()
        {
            // Arrange
            var token = GenerateAdminToken();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var gamne1 = new GameRequest { Title = "Super Mario", Category = "Plataforma" };
            var gamne2 = new GameRequest { Title = "FIFA 2026", Category = "Esporte" };

            await _client.PostAsJsonAsync("/games", gamne1);
            await _client.PostAsJsonAsync("/games", gamne2);

            // Act
            var response = await _client.GetAsync("/games");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var games = await response.Content.ReadFromJsonAsync<List<GameResponse>>();
            Assert.NotNull(games);
            Assert.Equal(2, games.Count);
        }

        [Fact]
        public async Task GetGameById_WhenGaaeExists_Returns200WithGame()
        {
            // Arrange
            var token = GenerateAdminToken();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var request = new GameRequest { Title = "Super Mario", Category = "Plataforma" };
            var createResponse = await _client.PostAsJsonAsync("/games", request);
            var createdGame = await createResponse.Content.ReadFromJsonAsync<GameResponse>();

            Assert.NotNull(createdGame);

            // Act
            var response = await _client.GetAsync($"/games/{createdGame.Id}");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var game = await response.Content.ReadFromJsonAsync<GameResponse>();
            Assert.NotNull(game);
            Assert.Equal(request.Title, game.Title);
            Assert.Equal(request.Category, game.Category);
        }

        [Fact]
        public async Task GetGameById_WhenGameDoesNotExist_Returns404()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();

            // Act
            var response = await _client.GetAsync($"/games/{nonExistentId}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task UpdateGame_WithValidDataAndAdminToken_Returns200()
        {
            // Arrange
            var token = GenerateAdminToken();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var request = new GameRequest { Title = "Super Mario", Category = "Plataforma" };
            var createResponse = await _client.PostAsJsonAsync("/games", request);
            var createdGame = await createResponse.Content.ReadFromJsonAsync<GameResponse>();

            var updateRequest = new GameRequest { Title = "Super Mario Updated", Category = "Plataforma" };

            Assert.NotNull(createdGame);

            // Act
            var response = await _client.PutAsJsonAsync($"/games/{createdGame.Id}", updateRequest);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var updatedGame = await response.Content.ReadFromJsonAsync<GameResponse>();
            Assert.NotNull(updatedGame);
            Assert.Equal(updateRequest.Title, updatedGame.Title);
            Assert.Equal(updateRequest.Category, updatedGame.Category);
        }

        [Fact]
        public async Task UpdateGame_WhenGameDoesNotExist_Returns404()
        {
            // Arrange
            var token = GenerateAdminToken();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var nonExistentId = Guid.NewGuid();
            var updateRequest = new GameRequest { Title = "Super Mario", Category = "Plataforma" };

            // Act
            var response = await _client.PutAsJsonAsync($"/games/{nonExistentId}", updateRequest);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task DeleteGame_WhenGameExistsAndAdminToken_Returns200()
        {
            // Arrange
            var token = GenerateAdminToken();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var request = new GameRequest { Title = "Super Mario", Category = "Plataforma" };
            var createResponse = await _client.PostAsJsonAsync("/games", request);
            var createdGame = await createResponse.Content.ReadFromJsonAsync<GameResponse>();

            Assert.NotNull(createdGame);

            // Act
            var response = await _client.DeleteAsync($"/games/{createdGame.Id}");

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task DeleteGame_WhenGameDoesNotExist_Returns404()
        {
            // Arrange
            var token = GenerateAdminToken();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var nonExistentId = Guid.NewGuid();

            // Act
            var response = await _client.DeleteAsync($"/games/{nonExistentId}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task UpdateGame_WithoutToken_Returns401()
        {
            // Arrange - sem token
            var request = new GameRequest { Title = "Super Mario", Category = "Plataforma" };

            // Act
            var response = await _client.PutAsJsonAsync($"/games/{Guid.NewGuid()}", request);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task DeleteGame_WithoutToken_Returns401()
        {
            //Arrange
            // Act
            var response = await _client.DeleteAsync($"/games/{Guid.NewGuid()}");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }
}
