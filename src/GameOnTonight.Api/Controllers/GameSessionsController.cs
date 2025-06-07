using GameOnTonight.Application.GameSessions.Commands;
using GameOnTonight.Application.GameSessions.Queries;
using GameOnTonight.Application.GameSessions.ViewModels;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameOnTonight.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // Applique l'authentification à toutes les actions du contrôleur
public class GameSessionsController : ControllerBase
{
    private readonly IMediator _mediator;

    public GameSessionsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("history")]
    public async Task<ActionResult<IEnumerable<GameSessionViewModel>>> GetHistory([FromQuery] int? count = null)
    {
        var result = await _mediator.Send(new GetSessionHistoryQuery(count));
        return Ok(result);
    }

    [HttpGet("game/{boardGameId:int}")]
    public async Task<ActionResult<IEnumerable<GameSessionViewModel>>> GetByGame(int boardGameId)
    {
        var result = await _mediator.Send(new GetSessionsByGameQuery(boardGameId));
        return Ok(result);
    }

    [HttpGet("counts")]
    public async Task<ActionResult<IDictionary<int, int>>> GetGamePlayCounts()
    {
        var result = await _mediator.Send(new GetGamePlayCountsQuery());
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<int>> Create(CreateGameSessionCommand command)
    {
        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id }, new { Id = id });
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<GameSessionViewModel>> GetById(int id)
    {
        // Nous n'avons pas encore implémenté cette requête - à ajouter
        return NotFound("Cette fonctionnalité n'est pas encore implémentée");
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> Update(int id, UpdateGameSessionCommand command)
    {
        if (id != command.Id)
            return BadRequest("L'ID de la route ne correspond pas à l'ID dans le corps de la requête.");
            
        var result = await _mediator.Send(command);
        
        if (!result)
            return NotFound();
            
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id)
    {
        var result = await _mediator.Send(new DeleteGameSessionCommand(id));
        
        if (!result)
            return NotFound();
            
        return NoContent();
    }
}
