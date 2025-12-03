using GameOnTonight.Application.GameTypes.Commands;
using GameOnTonight.Application.GameTypes.Queries;
using GameOnTonight.Application.GameTypes.ViewModels;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameOnTonight.Api.Controllers;

[ApiController]
[Authorize]
[Route("[controller]")]
public class GameTypesController : ControllerBase
{
    private readonly IMediator _mediator;

    public GameTypesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<GameTypeViewModel>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<GameTypeViewModel>>> GetAll()
    {
        var result = await _mediator.Send(new GetAllGameTypesQuery());
        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(GameTypeViewModel), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] CreateGameTypeCommand command)
    {
        var gameTypeViewModel = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetAll), gameTypeViewModel);
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        var deleted = await _mediator.Send(new DeleteGameTypeCommand(id));
        if (!deleted) return NotFound();
        return NoContent();
    }
}
