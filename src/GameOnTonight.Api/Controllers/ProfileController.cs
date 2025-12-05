using GameOnTonight.Application.Profiles.Commands;
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
    
    /// <summary>
    /// Retrieves the user's profile with statistics.
    /// </summary>
    [HttpGet]
    [Produces<ProfileViewModel>]
    public async Task<ActionResult<ProfileViewModel>> Get()
    {
        var profile = await _mediator.Send(new GetUserProfileQuery());
        return Ok(profile);
    }
    
    /// <summary>
    /// Updates the user's display name.
    /// </summary>
    [HttpPut]
    [Produces<ProfileViewModel>]
    public async Task<ActionResult<ProfileViewModel>> UpdateProfile(UpdateProfileCommand command)
    {
        var profile = await _mediator.Send(command);
        return Ok(profile);
    }
}
