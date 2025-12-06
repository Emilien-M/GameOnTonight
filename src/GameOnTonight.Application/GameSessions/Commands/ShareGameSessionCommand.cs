using FluentValidation;
using GameOnTonight.Application.GameSessions.ViewModels;
using GameOnTonight.Domain.Exceptions;
using GameOnTonight.Domain.Repositories;
using GameOnTonight.Domain.Services;
using Mediator;

namespace GameOnTonight.Application.GameSessions.Commands;

/// <summary>
/// Command to share a game session with a group.
/// </summary>
public sealed record ShareGameSessionCommand(int GameSessionId, int GroupId) : IRequest<GameSessionViewModel>;

public sealed class ShareGameSessionCommandValidator : AbstractValidator<ShareGameSessionCommand>
{
    public ShareGameSessionCommandValidator()
    {
        RuleFor(x => x.GameSessionId)
            .GreaterThan(0)
            .WithMessage("GameSessionId must be greater than 0");

        RuleFor(x => x.GroupId)
            .GreaterThan(0)
            .WithMessage("GroupId must be greater than 0");
    }
}

public sealed class ShareGameSessionCommandHandler : IRequestHandler<ShareGameSessionCommand, GameSessionViewModel>
{
    private readonly IGameSessionRepository _gameSessionRepository;
    private readonly IGroupRepository _groupRepository;
    private readonly ICurrentUserService _currentUserService;

    public ShareGameSessionCommandHandler(
        IGameSessionRepository gameSessionRepository,
        IGroupRepository groupRepository,
        ICurrentUserService currentUserService)
    {
        _gameSessionRepository = gameSessionRepository;
        _groupRepository = groupRepository;
        _currentUserService = currentUserService;
    }

    public async ValueTask<GameSessionViewModel> Handle(ShareGameSessionCommand request, CancellationToken cancellationToken)
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
            throw new ForbiddenException("You can only share your own game sessions");
        }

        // Verify the user is a member of the group
        var isMember = await _groupRepository.IsUserMemberAsync(request.GroupId, userId, cancellationToken);
        if (!isMember)
        {
            throw new ForbiddenException("You must be a member of the group to share a game session with it");
        }

        // Share the game session with the group
        gameSession.ShareWithGroup(request.GroupId);
        await _gameSessionRepository.UpdateAsync(gameSession, cancellationToken);

        return new GameSessionViewModel(gameSession, userId);
    }
}
