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
[Authorize] // Applique l'authentification à toutes les actions du contrôleur
[Produces("application/json")]
public class GameSessionsController : ControllerBase
{
    private readonly IMediator _mediator;

    public GameSessionsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("history")]
    [ProducesResponseType(typeof(IEnumerable<GameSessionViewModel>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<IEnumerable<GameSessionViewModel>>> GetHistory([FromQuery] int? count = null)
    {
        var result = await _mediator.Send(new GetSessionHistoryQuery(count));
        return Ok(result);
    }

    [HttpGet("game/{boardGameId:int}")]
    [ProducesResponseType(typeof(IEnumerable<GameSessionViewModel>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<IEnumerable<GameSessionViewModel>>> GetByGame(int boardGameId)
    {
        var result = await _mediator.Send(new GetSessionsByGameQuery(boardGameId));
        return Ok(result);
    }

    [HttpGet("counts")]
    [ProducesResponseType(typeof(IDictionary<string, int>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<IDictionary<string, int>>> GetGamePlayCounts()
    {
        var result = await _mediator.Send(new GetGamePlayCountsQuery());
        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(int), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<int>> Create(CreateGameSessionCommand command)
    {
        var id = await _mediator.Send(command);
        return Ok(id);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(GameSessionViewModel), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<GameSessionViewModel>> GetById(int id)
    {
        // Nous n'avons pas encore implémenté cette requête - à ajouter
        return NotFound("Cette fonctionnalité n'est pas encore implémentée");
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult> Update(int id, UpdateGameSessionCommand command)
    {
        if (id != command.Id)
            return BadRequest("L'ID de la route ne correspond pas à l'ID dans le corps de la requête.");
            
        var result = await _mediator.Send(command);
        
        if (!result)
            return NotFound();
            
        return Ok();
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult> Delete(int id)
    {
        var result = await _mediator.Send(new DeleteGameSessionCommand(id));
        
        if (!result)
            return NotFound();
            
        return Ok();
    }
}
