using GameOnTonight.Domain.Repositories;
using GameOnTonight.Domain.Services;
using Mediator;

namespace GameOnTonight.Application.GameSessions.Commands;

/// <summary>
/// Command pour mettre à jour une session de jeu existante
/// </summary>
public record UpdateGameSessionCommand(
    int Id,
    int BoardGameId,
    DateTime PlayedAt,
    int PlayerCount,
    string? Notes = null
) : IRequest<bool>;

/// <summary>
/// Handler pour UpdateGameSessionCommand
/// </summary>
public class UpdateGameSessionCommandHandler : IRequestHandler<UpdateGameSessionCommand, bool>
{
    private readonly IGameSessionRepository _gameSessionRepository;
    private readonly IBoardGameRepository _boardGameRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateGameSessionCommandHandler(
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

    public async ValueTask<bool> Handle(UpdateGameSessionCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId!;
        
        // Vérifier que la session de jeu existe et appartient à l'utilisateur
        var gameSession = await _gameSessionRepository.GetByIdAsync(request.Id, userId);
        if (gameSession == null)
        {
            return false;
        }
        
        // Vérifier que le jeu appartient à l'utilisateur si on change le jeu associé
        if (gameSession.BoardGameId != request.BoardGameId)
        {
            var boardGame = await _boardGameRepository.GetByIdAsync(request.BoardGameId, userId);
            if (boardGame == null)
            {
                throw new InvalidOperationException($"Le jeu avec l'ID {request.BoardGameId} n'a pas été trouvé dans votre collection.");
            }
        }

        // Mise à jour des propriétés
        gameSession.BoardGameId = request.BoardGameId;
        gameSession.PlayedAt = request.PlayedAt;
        gameSession.PlayerCount = request.PlayerCount;
        gameSession.Notes = request.Notes;

        var result = await _gameSessionRepository.UpdateAsync(gameSession);
        await _unitOfWork.SaveChangesAsync();

        return result;
    }
}