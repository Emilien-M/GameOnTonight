using GameOnTonight.Application.GameSessions.Commands;
using GameOnTonight.Application.GameSessions.Queries;
using GameOnTonight.Application.GameSessions.ViewModels;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace GameOnTonight.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
public class GameSessionsController : ControllerBase
{
    private readonly IMediator _mediator;

    public GameSessionsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("history")]
    [ProducesResponseType(typeof(IEnumerable<GameSessionViewModel>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<GameSessionViewModel>>> GetHistory([FromQuery] int? count = null)
    {
        var result = await _mediator.Send(new GetSessionHistoryQuery(count));
        return Ok(result);
    }

    [HttpGet("game/{boardGameId:int}")]
    [ProducesResponseType(typeof(IEnumerable<GameSessionViewModel>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<GameSessionViewModel>>> GetByGame(int boardGameId)
    {
        var result = await _mediator.Send(new GetSessionsByGameQuery(boardGameId));
        return Ok(result);
    }

    [HttpGet("counts")]
    [ProducesResponseType(typeof(IDictionary<string, int>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IDictionary<string, int>>> GetGamePlayCounts()
    {
        var result = await _mediator.Send(new GetGamePlayCountsQuery());
        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(GameSessionViewModel), StatusCodes.Status200OK)]
    public async Task<ActionResult<int>> Create(CreateGameSessionCommand command)
    {
        var gameSession = await _mediator.Send(command);
        return Ok(gameSession);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(GameSessionViewModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GameSessionViewModel>> GetById(int id)
    {
        // This feature has not been implemented yet.
        return NotFound("This feature has not been implemented yet.");
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(GameSessionViewModel),StatusCodes.Status200OK)]
    public async Task<ActionResult> Update(int id, UpdateGameSessionCommand command)
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
        await _mediator.Send(new DeleteGameSessionCommand(id));

        return Ok();
    }
}
