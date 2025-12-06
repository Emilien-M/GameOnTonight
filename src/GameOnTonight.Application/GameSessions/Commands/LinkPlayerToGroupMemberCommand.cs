using FluentValidation;
using GameOnTonight.Application.GameSessions.ViewModels;
using GameOnTonight.Domain.Exceptions;
using GameOnTonight.Domain.Repositories;
using GameOnTonight.Domain.Services;
using Mediator;

namespace GameOnTonight.Application.GameSessions.Commands;

/// <summary>
/// Command to link a game session player to a group member.
/// </summary>
public sealed record LinkPlayerToGroupMemberCommand(
    int GameSessionId,
    int PlayerId,
    int? GroupMemberId
) : IRequest<GameSessionPlayerViewModel>;

public sealed class LinkPlayerToGroupMemberCommandValidator : AbstractValidator<LinkPlayerToGroupMemberCommand>
{
    public LinkPlayerToGroupMemberCommandValidator()
    {
        RuleFor(x => x.GameSessionId)
            .GreaterThan(0)
            .WithMessage("GameSessionId must be greater than 0");

        RuleFor(x => x.PlayerId)
            .GreaterThan(0)
            .WithMessage("PlayerId must be greater than 0");

        RuleFor(x => x.GroupMemberId)
            .GreaterThan(0)
            .When(x => x.GroupMemberId.HasValue)
            .WithMessage("GroupMemberId must be greater than 0 when specified");
    }
}

public sealed class LinkPlayerToGroupMemberCommandHandler : IRequestHandler<LinkPlayerToGroupMemberCommand, GameSessionPlayerViewModel>
{
    private readonly IGameSessionRepository _gameSessionRepository;
    private readonly IGroupRepository _groupRepository;
    private readonly ICurrentUserService _currentUserService;

    public LinkPlayerToGroupMemberCommandHandler(
        IGameSessionRepository gameSessionRepository,
        IGroupRepository groupRepository,
        ICurrentUserService currentUserService)
    {
        _gameSessionRepository = gameSessionRepository;
        _groupRepository = groupRepository;
        _currentUserService = currentUserService;
    }

    public async ValueTask<GameSessionPlayerViewModel> Handle(LinkPlayerToGroupMemberCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId
            ?? throw new UnauthorizedAccessException("User not authenticated");

        // Get the game session with players
        var gameSession = await _gameSessionRepository.GetByIdWithPlayersAsync(request.GameSessionId, cancellationToken);
        if (gameSession is null)
        {
            throw new NotFoundException("GameSession", request.GameSessionId);
        }

        // Verify the user owns the game session or has access (it's shared with a group they belong to)
        var hasAccess = gameSession.UserId == userId;
        if (!hasAccess && gameSession.GroupId.HasValue)
        {
            hasAccess = await _groupRepository.IsUserMemberAsync(gameSession.GroupId.Value, userId, cancellationToken);
        }

        if (!hasAccess)
        {
            throw new ForbiddenException("You don't have access to this game session");
        }

        // Find the player
        var player = gameSession.Players.FirstOrDefault(p => p.Id == request.PlayerId);
        if (player is null)
        {
            throw new NotFoundException("GameSessionPlayer", request.PlayerId);
        }

        // If linking to a group member, verify they are in the same group
        if (request.GroupMemberId.HasValue)
        {
            if (!gameSession.GroupId.HasValue)
            {
                throw new ForbiddenException("Cannot link player to group member when session is not shared with a group");
            }

            // Get the group to verify the member belongs to it
            var group = await _groupRepository.GetByIdWithMembersAsync(gameSession.GroupId.Value, cancellationToken);
            if (group is null)
            {
                throw new NotFoundException("Group", gameSession.GroupId.Value);
            }

            var member = group.Members.FirstOrDefault(m => m.Id == request.GroupMemberId.Value);
            if (member is null)
            {
                throw new ForbiddenException("The specified group member does not belong to the session's group");
            }

            player.LinkToGroupMember(request.GroupMemberId.Value);
        }
        else
        {
            player.UnlinkFromGroupMember();
        }

        await _gameSessionRepository.UpdateAsync(gameSession, cancellationToken);

        return new GameSessionPlayerViewModel(player);
    }
}
