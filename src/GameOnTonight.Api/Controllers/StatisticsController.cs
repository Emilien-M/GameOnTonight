using GameOnTonight.Application.Statistics.Queries;
using GameOnTonight.Application.Statistics.ViewModels;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameOnTonight.Api.Controllers;

[ApiController]
[Authorize]
[Route("[controller]")]
public class StatisticsController : ControllerBase
{
    private readonly IMediator _mediator;

    public StatisticsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Récupère les statistiques de l'utilisateur.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(StatisticsViewModel), StatusCodes.Status200OK)]
    public async Task<ActionResult<StatisticsViewModel>> GetStatistics()
    {
        var result = await _mediator.Send(new GetStatisticsQuery());
        return Ok(result);
    }
}
