using GameOnTonight.Application.GameSessions.ViewModels;
using GameOnTonight.Domain.Exceptions;
using GameOnTonight.Domain.Repositories;
using GameOnTonight.Domain.Services;
using Mediator;

namespace GameOnTonight.Application.GameSessions.Commands;

/// <summary>
/// Command to update an existing game session.
/// </summary>
public record UpdateGameSessionCommand(
    int Id,
    int BoardGameId,
    DateTime PlayedAt,
    int PlayerCount,
    string? Notes = null
) : IRequest<GameSessionViewModel>;

/// <summary>
/// Handler for UpdateGameSessionCommand.
/// </summary>
public class UpdateGameSessionCommandHandler : IRequestHandler<UpdateGameSessionCommand, GameSessionViewModel>
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

    public async ValueTask<GameSessionViewModel> Handle(UpdateGameSessionCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId!;
        
        var gameSession = await _gameSessionRepository.GetByIdAsync(request.Id, userId);
        if (gameSession == null)
        {
            throw new DomainException("GameSessionNotFound", $"The game session with ID {request.Id} was not found.");
        }
        
        if (gameSession.BoardGameId != request.BoardGameId)
        {
            var boardGame = await _boardGameRepository.GetByIdAsync(request.BoardGameId, userId);
            if (boardGame == null)
            {
                throw new InvalidOperationException($"The game with ID {request.BoardGameId} was not found in your collection.");
            }
        }

        gameSession.BoardGameId = request.BoardGameId;
        gameSession.PlayedAt = request.PlayedAt;
        gameSession.PlayerCount = request.PlayerCount;
        gameSession.Notes = request.Notes;

        var result = await _gameSessionRepository.UpdateAsync(gameSession);
        await _unitOfWork.SaveChangesAsync();

        return new GameSessionViewModel(gameSession);
    }
}