using GameOnTonight.Domain.Repositories;
using GameOnTonight.Domain.Services;
using Mediator;

namespace GameOnTonight.Application.GameSessions.Commands;

/// <summary>
/// Command pour supprimer une session de jeu
/// </summary>
public record DeleteGameSessionCommand(int Id) : IRequest<bool>;

/// <summary>
/// Handler pour DeleteGameSessionCommand
/// </summary>
public class DeleteGameSessionCommandHandler : IRequestHandler<DeleteGameSessionCommand, bool>
{
    private readonly IGameSessionRepository _gameSessionRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteGameSessionCommandHandler(
        IGameSessionRepository gameSessionRepository,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork)
    {
        _gameSessionRepository = gameSessionRepository;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }

    public async ValueTask<bool> Handle(DeleteGameSessionCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId!;
        
        var gameSession = await _gameSessionRepository.GetByIdAsync(request.Id, userId);
        if (gameSession == null)
        {
            return false;
        }

        var result = await _gameSessionRepository.RemoveAsync(gameSession);
        await _unitOfWork.SaveChangesAsync();

        return result;
    }
}
