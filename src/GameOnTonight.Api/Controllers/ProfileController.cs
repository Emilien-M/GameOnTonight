using GameOnTonight.Application.Profiles.Queries;
using GameOnTonight.Application.Profiles.ViewModels;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameOnTonight.Api.Controllers;

[ApiController]
[Authorize]
[Route("[controller]")]
public class ProfileController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProfileController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [HttpGet]
    [Produces<ProfileViewModel>]
    public async Task<ActionResult<ProfileViewModel>> Get()
    {
        var profile = await _mediator.Send(new GetUserProfileQuery());
        return Ok(profile);
    }
}
