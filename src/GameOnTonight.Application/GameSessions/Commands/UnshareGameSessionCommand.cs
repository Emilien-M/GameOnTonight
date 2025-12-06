using FluentValidation;
using GameOnTonight.Application.GameSessions.ViewModels;
using GameOnTonight.Domain.Exceptions;
using GameOnTonight.Domain.Repositories;
using GameOnTonight.Domain.Services;
using Mediator;

namespace GameOnTonight.Application.GameSessions.Commands;

/// <summary>
/// Command to unshare a game session from a group (make it private again).
/// </summary>
public sealed record UnshareGameSessionCommand(int GameSessionId) : IRequest<GameSessionViewModel>;

public sealed class UnshareGameSessionCommandValidator : AbstractValidator<UnshareGameSessionCommand>
{
    public UnshareGameSessionCommandValidator()
    {
        RuleFor(x => x.GameSessionId)
            .GreaterThan(0)
            .WithMessage("GameSessionId must be greater than 0");
    }
}

public sealed class UnshareGameSessionCommandHandler : IRequestHandler<UnshareGameSessionCommand, GameSessionViewModel>
{
    private readonly IGameSessionRepository _gameSessionRepository;
    private readonly ICurrentUserService _currentUserService;

    public UnshareGameSessionCommandHandler(
        IGameSessionRepository gameSessionRepository,
        ICurrentUserService currentUserService)
    {
        _gameSessionRepository = gameSessionRepository;
        _currentUserService = currentUserService;
    }

    public async ValueTask<GameSessionViewModel> Handle(UnshareGameSessionCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId
            ?? throw new UnauthorizedAccessException("User not authenticated");

        // Get the game session with players
        var gameSession = await _gameSessionRepository.GetByIdWithPlayersAsync(request.GameSessionId, cancellationToken);
        if (gameSession is null)
        {
            throw new NotFoundException("GameSession", request.GameSessionId);
        }

        // Verify the user owns the game session
        if (gameSession.UserId != userId)
        {
            throw new ForbiddenException("You can only unshare your own game sessions");
        }

        // Make the game session private
        gameSession.MakePrivate();
        await _gameSessionRepository.UpdateAsync(gameSession, cancellationToken);

        return new GameSessionViewModel(gameSession, userId);
    }
}
