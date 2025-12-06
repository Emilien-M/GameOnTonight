using GameOnTonight.Application.Groups.ViewModels;
using GameOnTonight.Domain.Repositories;
using GameOnTonight.Domain.Services;
using Mediator;

namespace GameOnTonight.Application.Groups.Queries;

/// <summary>
/// Query to get all groups the current user is a member of.
/// </summary>
public sealed record GetMyGroupsQuery : IRequest<IEnumerable<GroupViewModel>>;

public sealed class GetMyGroupsQueryHandler : IRequestHandler<GetMyGroupsQuery, IEnumerable<GroupViewModel>>
{
    private readonly IGroupRepository _groupRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetMyGroupsQueryHandler(
        IGroupRepository groupRepository,
        ICurrentUserService currentUserService)
    {
        _groupRepository = groupRepository;
        _currentUserService = currentUserService;
    }

    public async ValueTask<IEnumerable<GroupViewModel>> Handle(
        GetMyGroupsQuery request, 
        CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId 
            ?? throw new UnauthorizedAccessException("User not authenticated");

        var groups = await _groupRepository.GetUserGroupsAsync(userId, cancellationToken);

        return groups.Select(g => new GroupViewModel(g, userId));
    }
}
