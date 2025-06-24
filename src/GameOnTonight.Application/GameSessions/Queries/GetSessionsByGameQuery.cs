using GameOnTonight.Application.GameSessions.ViewModels;
using GameOnTonight.Domain.Repositories;
using GameOnTonight.Domain.Services;
using Mediator;

namespace GameOnTonight.Application.GameSessions.Queries;

/// <summary>
/// Query to retrieve game sessions for a specific game.
/// </summary>
public record GetSessionsByGameQuery(int BoardGameId) : IRequest<IEnumerable<GameSessionViewModel>>;

/// <summary>
/// Handler for GetSessionsByGameQuery.
/// </summary>
public class GetSessionsByGameQueryHandler : IRequestHandler<GetSessionsByGameQuery, IEnumerable<GameSessionViewModel>>
{
    private readonly IGameSessionRepository _gameSessionRepository;
    private readonly IBoardGameRepository _boardGameRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetSessionsByGameQueryHandler(
        IGameSessionRepository gameSessionRepository,
        IBoardGameRepository boardGameRepository,
        ICurrentUserService currentUserService)
    {
        _gameSessionRepository = gameSessionRepository;
        _boardGameRepository = boardGameRepository;
        _currentUserService = currentUserService;
    }

    public async ValueTask<IEnumerable<GameSessionViewModel>> Handle(GetSessionsByGameQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId!;
        
        var boardGame = await _boardGameRepository.GetByIdAsync(request.BoardGameId, userId);
        if (boardGame == null)
        {
            throw new InvalidOperationException($"The game with ID {request.BoardGameId} was not found in your collection.");
        }
        
        var sessions = await _gameSessionRepository.GetSessionsByGameAsync(request.BoardGameId, userId);
        
        return sessions.Select(s => new GameSessionViewModel(s));
    }
}