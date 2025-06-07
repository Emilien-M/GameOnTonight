using GameOnTonight.Domain.Repositories;
using GameOnTonight.Domain.Services;
using Mediator;

namespace GameOnTonight.Application.GameSessions.Queries;

/// <summary>
/// Query pour obtenir le nombre de parties jouées pour chaque jeu
/// </summary>
public record GetGamePlayCountsQuery : IRequest<IDictionary<int, int>>;

/// <summary>
/// Handler pour GetGamePlayCountsQuery
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