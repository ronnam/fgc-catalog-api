using Fgc.Catalog.Application.DTOS.Games;
using Fgc.Catalog.Application.Services;
using Fgc.Catalog.Domain.Entities;
using Fgc.Catalog.Domain.Exceptions;
using Fgc.Catalog.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fgc.Catalog.API.Controllers;

[ApiController]
[Route("games")]
[Tags("Games")]
public class GameController : ControllerBase
{
    private readonly GameService _gameService;

    public GameController(GameService gameService)
    {
        _gameService = gameService;
    }

    /// <summary>
    /// Create a new game (only Admin)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateGame([FromBody] GameRequest request)    
    {
        try
        {
            var response = await _gameService.CreateAsync(request);

            return CreatedAtAction(nameof(GetGameById), new { id = response.Id }, response);
        }
        catch (CatalogDomainException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Get all games
    /// </summary>

    [HttpGet]
    public async Task<IActionResult> GetAllGames()
    {
        var games = await _gameService.GetAllAsync();
        return Ok(games);
    }

    /// <summary>
    /// Get a game by ID
    /// </summary>

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetGameById(Guid id)
    {
        var game = await _gameService.GetByIdAsync(id);
        if (game == null)
            return NotFound(new {error = "Game not found"});

        return Ok(game);
    }

    /// <summary>
    ///  Update a game(only Admin)
    ///  </summary>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateGame(Guid id, [FromBody] GameRequest request)
    {
        try
        {
            var response = await _gameService.UpdateAsync(id, request);
            return Ok(response);
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (CatalogDomainException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Delete a game (only Admin)
    /// </summary>  

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteGame(Guid id)
    {
        try
        {
            await _gameService.DeleteAsync(id);
            return NoContent();
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (CatalogDomainException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}
