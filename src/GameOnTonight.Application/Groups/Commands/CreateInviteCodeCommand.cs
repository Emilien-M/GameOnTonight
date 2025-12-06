using FluentValidation;
using GameOnTonight.Application.Groups.ViewModels;
using GameOnTonight.Domain.Exceptions;
using GameOnTonight.Domain.Repositories;
using GameOnTonight.Domain.Services;
using Mediator;

namespace GameOnTonight.Application.Groups.Commands;

public sealed record CreateInviteCodeCommand(int GroupId) : IRequest<GroupInviteCodeViewModel>;

public sealed class CreateInviteCodeCommandValidator : AbstractValidator<CreateInviteCodeCommand>
{
    public CreateInviteCodeCommandValidator()
    {
        RuleFor(x => x.GroupId).GreaterThan(0);
    }
}

public sealed class CreateInviteCodeCommandHandler : IRequestHandler<CreateInviteCodeCommand, GroupInviteCodeViewModel>
{
    private readonly IGroupRepository _groupRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly TimeProvider _timeProvider;

    public CreateInviteCodeCommandHandler(
        IGroupRepository groupRepository,
        ICurrentUserService currentUserService, 
        TimeProvider timeProvider)
    {
        _groupRepository = groupRepository;
        _currentUserService = currentUserService;
        _timeProvider = timeProvider;
    }

    public async ValueTask<GroupInviteCodeViewModel> Handle(
        CreateInviteCodeCommand request, 
        CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId 
            ?? throw new UnauthorizedAccessException("User not authenticated");

        var group = await _groupRepository.GetByIdWithMembersAsync(request.GroupId, cancellationToken)
            ?? throw new NotFoundException($"Group with ID {request.GroupId} not found");

        var inviteCode = group.CreateInviteCode(userId, _timeProvider.GetUtcNow().UtcDateTime);

        if (group.HasErrors || inviteCode is null)
        {
            var error = group.DomainErrors.First();
            throw new DomainException(error.Name, error.Message);
        }

        await _groupRepository.UpdateAsync(group, cancellationToken);

        return new GroupInviteCodeViewModel(inviteCode);
    }
}
