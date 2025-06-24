using GameOnTonight.Domain.Repositories;
using GameOnTonight.Domain.Services;
using Mediator;

namespace GameOnTonight.Application.GameSessions.Queries;

/// <summary>
/// Query to get the number of games played for each game.
/// </summary>
public record GetGamePlayCountsQuery : IRequest<IDictionary<int, int>>;

/// <summary>
/// Handler for GetGamePlayCountsQuery.
/// </summary>
public class GetGamePlayCountsQueryHandler : IRequestHandler<GetGamePlayCountsQuery, IDictionary<int, int>>
{
    private readonly IGameSessionRepository _gameSessionRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetGamePlayCountsQueryHandler(
        IGameSessionRepository gameSessionRepository,
        ICurrentUserService currentUserService)
    {
        _gameSessionRepository = gameSessionRepository;
        _currentUserService = currentUserService;
    }

    public async ValueTask<IDictionary<int, int>> Handle(GetGamePlayCountsQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId!;
        
        return await _gameSessionRepository.GetGamePlayCountsAsync(userId);
    }
}