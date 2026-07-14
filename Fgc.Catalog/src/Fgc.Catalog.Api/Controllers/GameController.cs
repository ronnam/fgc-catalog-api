using Fgc.Catalog.Application.DTOS.Games;
using Fgc.Catalog.Application.Services;
using Fgc.Catalog.Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fgc.Catalog.API.Controllers;

[ApiController]
[Route("games")]
[Tags("Games")]
public class GameController(GameService gameService) : ControllerBase
{
    
    /// <summary>
    /// Create a new game (only Admin)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateGame([FromBody] GameRequest request)    
    {
        try
        {
            var response = await gameService.CreateAsync(request);

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
        var games = await gameService.GetAllAsync();
        return Ok(games);
    }

    /// <summary>
    /// Get a game by ID
    /// </summary>

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetGameById(Guid id)
    {
        var game = await gameService.GetByIdAsync(id);
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
            var response = await gameService.UpdateAsync(id, request);
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
            await gameService.DeleteAsync(id);
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
