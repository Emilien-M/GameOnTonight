using GameOnTonight.Application.GameSessions.ViewModels;
using GameOnTonight.Domain.Entities;
using GameOnTonight.Domain.Repositories;
using GameOnTonight.Domain.Services;
using Mediator;

namespace GameOnTonight.Application.GameSessions.Commands;

/// <summary>
/// Command to create a new game session.
/// </summary>
public record CreateGameSessionCommand(
    int BoardGameId,
    DateTime PlayedAt,
    int PlayerCount,
    string? Notes = null
) : IRequest<GameSessionViewModel>;

/// <summary>
/// Handler for CreateGameSessionCommand.
/// </summary>
public class CreateGameSessionCommandHandler : IRequestHandler<CreateGameSessionCommand, GameSessionViewModel>
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

    public async ValueTask<GameSessionViewModel> Handle(CreateGameSessionCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId!;

        var boardGame = await _boardGameRepository.GetByIdAsync(request.BoardGameId, userId);
        if (boardGame == null)
        {
            throw new InvalidOperationException($"The game with ID {request.BoardGameId} was not found in your collection.");
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

        return new GameSessionViewModel(gameSession);
    }
}