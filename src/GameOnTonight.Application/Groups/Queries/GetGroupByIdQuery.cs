using FluentValidation;
using GameOnTonight.Application.Groups.ViewModels;
using GameOnTonight.Domain.Exceptions;
using GameOnTonight.Domain.Repositories;
using GameOnTonight.Domain.Services;
using Mediator;

namespace GameOnTonight.Application.Groups.Queries;

/// <summary>
/// Query to get a group by its ID with full details.
/// </summary>
public sealed record GetGroupByIdQuery(int Id) : IRequest<GroupDetailViewModel>;

public sealed class GetGroupByIdQueryValidator : AbstractValidator<GetGroupByIdQuery>
{
    public GetGroupByIdQueryValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
    }
}

public sealed class GetGroupByIdQueryHandler : IRequestHandler<GetGroupByIdQuery, GroupDetailViewModel>
{
    private readonly IGroupRepository _groupRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly TimeProvider _timeProvider;

    public GetGroupByIdQueryHandler(
        IGroupRepository groupRepository,
        ICurrentUserService currentUserService,
        TimeProvider timeProvider)
    {
        _groupRepository = groupRepository;
        _currentUserService = currentUserService;
        _timeProvider = timeProvider;
    }

    public async ValueTask<GroupDetailViewModel> Handle(
        GetGroupByIdQuery request, 
        CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId 
            ?? throw new UnauthorizedAccessException("User not authenticated");

        var group = await _groupRepository.GetByIdWithMembersAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException($"Group with ID {request.Id} not found");

        // Verify user is a member
        if (!group.IsMember(userId))
            throw new ForbiddenException("You are not a member of this group");

        return new GroupDetailViewModel(group, userId, _timeProvider.GetUtcNow().UtcDateTime);
    }
}
