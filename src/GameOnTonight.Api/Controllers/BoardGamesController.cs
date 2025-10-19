using GameOnTonight.Application.BoardGames.Commands;
using GameOnTonight.Application.BoardGames.Queries;
using GameOnTonight.Application.BoardGames.ViewModels;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameOnTonight.Api.Controllers;

[ApiController]
[Authorize]
[Route("[controller]")]
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
    public async Task<ActionResult<BoardGameViewModel>> GetById([FromRoute] int id)
    {
        var vm = await _mediator.Send(new GetBoardGameByIdQuery(id));
        if (vm is null) return NotFound();
        return Ok(vm);
    }

    [HttpPost]
    [ProducesResponseType(typeof(BoardGameViewModel), StatusCodes.Status200OK)]
    public async Task<IActionResult> Create([FromBody] CreateBoardGameCommand command)
    {
        var boardGameViewModel = await _mediator.Send(command);
        return Ok(boardGameViewModel);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(BoardGameViewModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BoardGameViewModel>> Update([FromRoute] int id, [FromBody] UpdateBoardGameCommand command)
    {
        if (id != command.Id) return BadRequest();
        var vm = await _mediator.Send(command);
        if (vm is null) return NotFound();
        return Ok(vm);
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        var success = await _mediator.Send(new DeleteBoardGameCommand(id));
        if (!success) return NotFound();
        return NoContent();
    }
}
