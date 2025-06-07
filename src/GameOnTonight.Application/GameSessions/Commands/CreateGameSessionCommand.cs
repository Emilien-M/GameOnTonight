using GameOnTonight.Domain.Entities;
using GameOnTonight.Domain.Repositories;
using GameOnTonight.Domain.Services;
using Mediator;

namespace GameOnTonight.Application.GameSessions.Commands;

/// <summary>
/// Command pour créer une nouvelle session de jeu
/// </summary>
public record CreateGameSessionCommand(
    int BoardGameId,
    DateTime PlayedAt,
    int PlayerCount,
    string? Notes = null
) : IRequest<int>;

/// <summary>
/// Handler pour CreateGameSessionCommand
/// </summary>
public class CreateGameSessionCommandHandler : IRequestHandler<CreateGameSessionCommand, int>
{
    private readonly IGameSessionRepository _gameSessionRepository;
    private readonly IBoardGameRepository _boardGameRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public CreateGameSessionCommandHandler(
        IGameSessionRepository gameSessionRepository,
        IBoardGameRepository boardGameRepository,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork)
    {
        _gameSessionRepository = gameSessionRepository;
        _boardGameRepository = boardGameRepository;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }

    public async ValueTask<int> Handle(CreateGameSessionCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId!;

        // Vérifier que le jeu appartient à l'utilisateur
        var boardGame = await _boardGameRepository.GetByIdAsync(request.BoardGameId, userId);
        if (boardGame == null)
        {
            throw new InvalidOperationException($"Le jeu avec l'ID {request.BoardGameId} n'a pas été trouvé dans votre collection.");
        }

        var gameSession = new GameSession
        {
            BoardGameId = request.BoardGameId,
            PlayedAt = request.PlayedAt,
            PlayerCount = request.PlayerCount,
            Notes = request.Notes
        };

        await _gameSessionRepository.AddAsync(gameSession);
        await _unitOfWork.SaveChangesAsync();

        return gameSession.Id;
    }
}