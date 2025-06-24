using GameOnTonight.Application.GameSessions.ViewModels;
using GameOnTonight.Domain.Repositories;
using GameOnTonight.Domain.Services;
using Mediator;

namespace GameOnTonight.Application.GameSessions.Queries;

/// <summary>
/// Query to retrieve the history of game sessions.
/// </summary>
public record GetSessionHistoryQuery(int? Count = null) : IRequest<IEnumerable<GameSessionViewModel>>;

/// <summary>
/// Handler for GetSessionHistoryQuery.
/// </summary>
public class GetSessionHistoryQueryHandler : IRequestHandler<GetSessionHistoryQuery, IEnumerable<GameSessionViewModel>>
{
    private readonly IGameSessionRepository _gameSessionRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetSessionHistoryQueryHandler(
        IGameSessionRepository gameSessionRepository,
        ICurrentUserService currentUserService)
    {
        _gameSessionRepository = gameSessionRepository;
        _currentUserService = currentUserService;
    }

    public async ValueTask<IEnumerable<GameSessionViewModel>> Handle(GetSessionHistoryQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId!;
        
        var sessions = await _gameSessionRepository.GetSessionHistoryAsync(userId, request.Count);
        
        return sessions.Select(s => new GameSessionViewModel(s));
    }
}