using GameOnTonight.Application.GameSessions.ViewModels;
using GameOnTonight.Domain.Repositories;
using GameOnTonight.Domain.Services;
using Mediator;

namespace GameOnTonight.Application.GameSessions.Queries;

/// <summary>
/// Query pour récupérer les sessions de jeu pour un jeu spécifique
/// </summary>
public record GetSessionsByGameQuery(int BoardGameId) : IRequest<IEnumerable<GameSessionViewModel>>;

/// <summary>
/// Handler pour GetSessionsByGameQuery
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
        
        // Vérifier que le jeu appartient à l'utilisateur
        var boardGame = await _boardGameRepository.GetByIdAsync(request.BoardGameId, userId);
        if (boardGame == null)
        {
            throw new InvalidOperationException($"Le jeu avec l'ID {request.BoardGameId} n'a pas été trouvé dans votre collection.");
        }
        
        var sessions = await _gameSessionRepository.GetSessionsByGameAsync(request.BoardGameId, userId);
        
        return sessions.Select(s => new GameSessionViewModel(s));
    }
}