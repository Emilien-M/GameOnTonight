using FluentValidation;
using GameOnTonight.Application.Groups.ViewModels;
using GameOnTonight.Domain.Exceptions;
using GameOnTonight.Domain.Repositories;
using GameOnTonight.Domain.Services;
using Mediator;

namespace GameOnTonight.Application.Groups.Queries;

/// <summary>
/// Query to get all members of a group.
/// </summary>
public sealed record GetGroupMembersQuery(int GroupId) : IRequest<IEnumerable<GroupMemberViewModel>>;

public sealed class GetGroupMembersQueryValidator : AbstractValidator<GetGroupMembersQuery>
{
    public GetGroupMembersQueryValidator()
    {
        RuleFor(x => x.GroupId).GreaterThan(0);
    }
}

public sealed class GetGroupMembersQueryHandler : IRequestHandler<GetGroupMembersQuery, IEnumerable<GroupMemberViewModel>>
{
    private readonly IGroupRepository _groupRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetGroupMembersQueryHandler(
        IGroupRepository groupRepository,
        ICurrentUserService currentUserService)
    {
        _groupRepository = groupRepository;
        _currentUserService = currentUserService;
    }

    public async ValueTask<IEnumerable<GroupMemberViewModel>> Handle(
        GetGroupMembersQuery request, 
        CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId 
            ?? throw new UnauthorizedAccessException("User not authenticated");

        var group = await _groupRepository.GetByIdWithMembersAsync(request.GroupId, cancellationToken)
            ?? throw new NotFoundException($"Group with ID {request.GroupId} not found");

        // Verify user is a member
        if (!group.IsMember(userId))
            throw new ForbiddenException("You are not a member of this group");

        return group.Members
            .OrderByDescending(m => m.Role)
            .ThenBy(m => m.JoinedAt)
            .Select(m => new GroupMemberViewModel(m));
    }
}
