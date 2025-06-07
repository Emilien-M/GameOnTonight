using GameOnTonight.Application.Auth.Commands;
using GameOnTonight.Application.Auth.Queries;
using GameOnTonight.Application.Auth.ViewModels;
using Mediator;
using Microsoft.AspNetCore.Mvc;

namespace GameOnTonight.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserCommand command, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        await _mediator.Send(command, cancellationToken);

        return Ok(new { message = "Utilisateur créé avec succès" });
    }

    [HttpPost("login")]
    [Produces(typeof(TokenViewModel))]
    public async Task<IActionResult> Login([FromBody] LoginQuery query, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _mediator.Send(query, cancellationToken);

        return Ok(result);
    }
}