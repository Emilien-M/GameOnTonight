using FluentValidation;
using GameOnTonight.Application.Groups.ViewModels;
using GameOnTonight.Domain.Exceptions;
using GameOnTonight.Domain.Repositories;
using GameOnTonight.Domain.Services;
using Mediator;

namespace GameOnTonight.Application.Groups.Commands;

public sealed record JoinGroupCommand(string InviteCode) : IRequest<GroupViewModel>;

public sealed class JoinGroupCommandValidator : AbstractValidator<JoinGroupCommand>
{
    public JoinGroupCommandValidator()
    {
        RuleFor(x => x.InviteCode)
            .NotEmpty().WithMessage("Invite code is required")
            .Length(16).WithMessage("Invite code must be 16 characters");
    }
}

public sealed class JoinGroupCommandHandler : IRequestHandler<JoinGroupCommand, GroupViewModel>
{
    private readonly IGroupRepository _groupRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly TimeProvider _timeProvider;

    public JoinGroupCommandHandler(
        IGroupRepository groupRepository,
        ICurrentUserService currentUserService, 
        TimeProvider timeProvider)
    {
        _groupRepository = groupRepository;
        _currentUserService = currentUserService;
        _timeProvider = timeProvider;
    }

    public async ValueTask<GroupViewModel> Handle(
        JoinGroupCommand request, 
        CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId 
            ?? throw new UnauthorizedAccessException("User not authenticated");

        var group = await _groupRepository.GetByInviteCodeAsync(request.InviteCode, cancellationToken)
            ?? throw new NotFoundException("Invalid or expired invite code");

        group.AddMember(userId, _timeProvider.GetUtcNow().UtcDateTime);

        if (group.HasErrors)
        {
            var error = group.DomainErrors.First();
            throw new DomainException(error.Name, error.Message);
        }

        await _groupRepository.UpdateAsync(group, cancellationToken);

        return new GroupViewModel(group, userId);
    }
}
