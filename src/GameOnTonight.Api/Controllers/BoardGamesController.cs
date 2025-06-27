using GameOnTonight.Application.BoardGames.Commands;
using GameOnTonight.Application.BoardGames.Queries;
using GameOnTonight.Application.BoardGames.ViewModels;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace GameOnTonight.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
public class BoardGamesController : ControllerBase
{
    private readonly IMediator _mediator;

    public BoardGamesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<BoardGameViewModel>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<BoardGameViewModel>>> GetAll()
    {
        var result = await _mediator.Send(new GetAllBoardGamesQuery());
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(BoardGameViewModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BoardGameViewModel>> GetById(int id)
    {
        var result = await _mediator.Send(new GetBoardGameByIdQuery(id));
        
        if (result == null)
            return NotFound();
            
        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(BoardGameViewModel), StatusCodes.Status200OK)]
    public async Task<ActionResult<int>> Create(CreateBoardGameCommand command)
    {
        var id = await _mediator.Send(command);
        return Ok(id);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(BoardGameViewModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Update(int id, UpdateBoardGameCommand command)
    {
        if (id != command.Id)
            return BadRequest("The ID in the route does not match the ID in the request body.");
            
        var result = await _mediator.Send(command);
            
        return Ok(result);
    }

    [HttpDelete("{id:int}")]
    [ProducesDefaultResponseType]
    public async Task<ActionResult> Delete(int id)
    {
        await _mediator.Send(new DeleteBoardGameCommand(id));
            
        return Ok();
    }

    [HttpGet("filter")]
    [ProducesResponseType(typeof(IEnumerable<BoardGameViewModel>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<BoardGameViewModel>>> Filter([FromQuery] int playerCount, [FromQuery] int maxDuration, [FromQuery] string? gameType = null)
    {
        var result = await _mediator.Send(new FilterBoardGamesQuery(playerCount, maxDuration, gameType));
        return Ok(result);
    }

    [HttpGet("types")]
    [ProducesResponseType(typeof(IEnumerable<string>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<string>>> GetDistinctTypes()
    {
        var result = await _mediator.Send(new GetDistinctGameTypesQuery());
        return Ok(result);
    }

    [HttpPost("random")]
    [ProducesResponseType(typeof(BoardGameViewModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BoardGameViewModel>> GetRandom([FromBody] IEnumerable<int> gameIds)
    {
        var result = await _mediator.Send(new GetRandomBoardGameQuery(gameIds));
        
        if (result == null)
            return NotFound();
            
        return Ok(result);
    }
}
