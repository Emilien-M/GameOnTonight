using FluentValidation;
using GameOnTonight.Domain.Exceptions;
using GameOnTonight.Domain.Repositories;
using GameOnTonight.Domain.Services;
using Mediator;

namespace GameOnTonight.Application.Groups.Commands;

public sealed record RevokeInviteCodeCommand(int GroupId, int InviteCodeId) : IRequest<Unit>;

public sealed class RevokeInviteCodeCommandValidator : AbstractValidator<RevokeInviteCodeCommand>
{
    public RevokeInviteCodeCommandValidator()
    {
        RuleFor(x => x.GroupId).GreaterThan(0);
        RuleFor(x => x.InviteCodeId).GreaterThan(0);
    }
}

public sealed class RevokeInviteCodeCommandHandler : IRequestHandler<RevokeInviteCodeCommand, Unit>
{
    private readonly IGroupRepository _groupRepository;
    private readonly ICurrentUserService _currentUserService;

    public RevokeInviteCodeCommandHandler(
        IGroupRepository groupRepository,
        ICurrentUserService currentUserService)
    {
        _groupRepository = groupRepository;
        _currentUserService = currentUserService;
    }

    public async ValueTask<Unit> Handle(
        RevokeInviteCodeCommand request, 
        CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId 
            ?? throw new UnauthorizedAccessException("User not authenticated");

        var group = await _groupRepository.GetByIdWithMembersAsync(request.GroupId, cancellationToken)
            ?? throw new NotFoundException($"Group with ID {request.GroupId} not found");

        group.RevokeInviteCode(request.InviteCodeId, userId);

        if (group.HasErrors)
        {
            var error = group.DomainErrors.First();
            throw new DomainException(error.Name, error.Message);
        }

        await _groupRepository.UpdateAsync(group, cancellationToken);

        return Unit.Value;
    }
}
