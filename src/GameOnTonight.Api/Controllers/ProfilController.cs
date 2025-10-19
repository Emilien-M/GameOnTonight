using GameOnTonight.Application.Profils.Queries;
using GameOnTonight.Application.Profils.ViewModels;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameOnTonight.Api.Controllers;

[ApiController]
[Authorize]
[Route("[controller]")]
public class ProfilController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProfilController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [HttpGet]
    [Produces<ProfilViewModel>]
    public async Task<ActionResult<ProfilViewModel>> Get()
    {
        var profil = await _mediator.Send(new GetUserProfilQuery());
        return Ok(profil);
    }
}