using FluentValidation;
using GameOnTonight.Application.Groups.ViewModels;
using GameOnTonight.Domain.Exceptions;
using GameOnTonight.Domain.Repositories;
using GameOnTonight.Domain.Services;
using Mediator;

namespace GameOnTonight.Application.Groups.Commands;

public sealed record TransferOwnershipCommand(int GroupId, string NewOwnerId) : IRequest<GroupViewModel>;

public sealed class TransferOwnershipCommandValidator : AbstractValidator<TransferOwnershipCommand>
{
    public TransferOwnershipCommandValidator()
    {
        RuleFor(x => x.GroupId).GreaterThan(0);
        RuleFor(x => x.NewOwnerId).NotEmpty();
    }
}

public sealed class TransferOwnershipCommandHandler : IRequestHandler<TransferOwnershipCommand, GroupViewModel>
{
    private readonly IGroupRepository _groupRepository;
    private readonly ICurrentUserService _currentUserService;

    public TransferOwnershipCommandHandler(
        IGroupRepository groupRepository,
        ICurrentUserService currentUserService)
    {
        _groupRepository = groupRepository;
        _currentUserService = currentUserService;
    }

    public async ValueTask<GroupViewModel> Handle(
        TransferOwnershipCommand request, 
        CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId 
            ?? throw new UnauthorizedAccessException("User not authenticated");

        var group = await _groupRepository.GetByIdWithMembersAsync(request.GroupId, cancellationToken)
            ?? throw new NotFoundException($"Group with ID {request.GroupId} not found");

        group.TransferOwnership(userId, request.NewOwnerId);

        if (group.HasErrors)
        {
            var error = group.DomainErrors.First();
            throw new DomainException(error.Name, error.Message);
        }

        await _groupRepository.UpdateAsync(group, cancellationToken);

        return new GroupViewModel(group, userId);
    }
}
