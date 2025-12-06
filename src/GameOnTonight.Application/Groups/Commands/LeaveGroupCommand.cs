using FluentValidation;
using GameOnTonight.Domain.Exceptions;
using GameOnTonight.Domain.Repositories;
using GameOnTonight.Domain.Services;
using Mediator;

namespace GameOnTonight.Application.Groups.Commands;

public sealed record LeaveGroupCommand(int GroupId) : IRequest<Unit>;

public sealed class LeaveGroupCommandValidator : AbstractValidator<LeaveGroupCommand>
{
    public LeaveGroupCommandValidator()
    {
        RuleFor(x => x.GroupId).GreaterThan(0);
    }
}

public sealed class LeaveGroupCommandHandler : IRequestHandler<LeaveGroupCommand, Unit>
{
    private readonly IGroupRepository _groupRepository;
    private readonly ICurrentUserService _currentUserService;

    public LeaveGroupCommandHandler(
        IGroupRepository groupRepository,
        ICurrentUserService currentUserService)
    {
        _groupRepository = groupRepository;
        _currentUserService = currentUserService;
    }

    public async ValueTask<Unit> Handle(
        LeaveGroupCommand request, 
        CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId 
            ?? throw new UnauthorizedAccessException("User not authenticated");

        var group = await _groupRepository.GetByIdWithMembersAsync(request.GroupId, cancellationToken)
            ?? throw new NotFoundException($"Group with ID {request.GroupId} not found");

        group.RemoveMember(userId);

        if (group.HasErrors)
        {
            var error = group.DomainErrors.First();
            throw new DomainException(error.Name, error.Message);
        }

        await _groupRepository.UpdateAsync(group, cancellationToken);

        return Unit.Value;
    }
}
