using FluentValidation;
using GameOnTonight.Application.Groups.ViewModels;
using GameOnTonight.Domain.Exceptions;
using GameOnTonight.Domain.Repositories;
using GameOnTonight.Domain.Services;
using Mediator;

namespace GameOnTonight.Application.Groups.Queries;

/// <summary>
/// Query to get all active invite codes for a group.
/// Only the owner can see invite codes.
/// </summary>
public sealed record GetGroupInviteCodesQuery(int GroupId) : IRequest<IEnumerable<GroupInviteCodeViewModel>>;

public sealed class GetGroupInviteCodesQueryValidator : AbstractValidator<GetGroupInviteCodesQuery>
{
    public GetGroupInviteCodesQueryValidator()
    {
        RuleFor(x => x.GroupId).GreaterThan(0);
    }
}

public sealed class GetGroupInviteCodesQueryHandler : IRequestHandler<GetGroupInviteCodesQuery, IEnumerable<GroupInviteCodeViewModel>>
{
    private readonly IGroupRepository _groupRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly TimeProvider _timeProvider;

    public GetGroupInviteCodesQueryHandler(
        IGroupRepository groupRepository,
        ICurrentUserService currentUserService, 
        TimeProvider timeProvider)
    {
        _groupRepository = groupRepository;
        _currentUserService = currentUserService;
        _timeProvider = timeProvider;
    }

    public async ValueTask<IEnumerable<GroupInviteCodeViewModel>> Handle(
        GetGroupInviteCodesQuery request, 
        CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId 
            ?? throw new UnauthorizedAccessException("User not authenticated");

        var group = await _groupRepository.GetByIdWithMembersAsync(request.GroupId, cancellationToken)
            ?? throw new NotFoundException($"Group with ID {request.GroupId} not found");

        // Only owner can see invite codes
        if (!group.IsOwner(userId))
            throw new ForbiddenException("Only the owner can view invite codes");

        return group.InviteCodes
            .Where(c => c.IsValid(_timeProvider.GetUtcNow().UtcDateTime))
            .OrderByDescending(c => c.CreatedAt)
            .Select(c => new GroupInviteCodeViewModel(c));
    }
}
