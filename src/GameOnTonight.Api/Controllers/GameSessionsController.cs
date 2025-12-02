using GameOnTonight.Application.GameSessions.Commands;
using GameOnTonight.Application.GameSessions.Queries;
using GameOnTonight.Application.GameSessions.ViewModels;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameOnTonight.Api.Controllers;

[ApiController]
[Authorize]
[Route("[controller]")]
public class GameSessionsController : ControllerBase
{
    private readonly IMediator _mediator;

    public GameSessionsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Récupère l'historique des parties de l'utilisateur.
    /// </summary>
    /// <param name="count">Nombre maximum de parties à récupérer (optionnel).</param>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<GameSessionViewModel>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<GameSessionViewModel>>> GetHistory([FromQuery] int? count = null)
    {
        var result = await _mediator.Send(new GetSessionHistoryQuery(count));
        return Ok(result);
    }

    /// <summary>
    /// Crée une nouvelle session de jeu.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(GameSessionViewModel), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateGameSessionCommand command)
    {
        var gameSessionViewModel = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetHistory), null, gameSessionViewModel);
    }

    /// <summary>
    /// Met à jour une session de jeu existante.
    /// </summary>
    /// <param name="id">ID de la session à mettre à jour.</param>
    /// <param name="command">Données de mise à jour.</param>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(GameSessionViewModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateGameSessionCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest("L'ID de la route ne correspond pas à l'ID de la commande.");
        }

        var result = await _mediator.Send(command);
        if (result is null) return NotFound();
        return Ok(result);
    }

    /// <summary>
    /// Supprime une session de jeu.
    /// </summary>
    /// <param name="id">ID de la session à supprimer.</param>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        var success = await _mediator.Send(new DeleteGameSessionCommand(id));
        if (!success) return NotFound();
        return NoContent();
    }
}
