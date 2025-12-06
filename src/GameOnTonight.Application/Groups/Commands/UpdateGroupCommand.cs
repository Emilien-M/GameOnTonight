using FluentValidation;
using GameOnTonight.Application.Groups.ViewModels;
using GameOnTonight.Domain.Exceptions;
using GameOnTonight.Domain.Repositories;
using GameOnTonight.Domain.Services;
using Mediator;

namespace GameOnTonight.Application.Groups.Commands;

public sealed record UpdateGroupCommand(
    int Id,
    string Name,
    string? Description) : IRequest<GroupViewModel>;

public sealed class UpdateGroupCommandValidator : AbstractValidator<UpdateGroupCommand>
{
    public UpdateGroupCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description must not exceed 500 characters");
    }
}

public sealed class UpdateGroupCommandHandler : IRequestHandler<UpdateGroupCommand, GroupViewModel>
{
    private readonly IGroupRepository _groupRepository;
    private readonly ICurrentUserService _currentUserService;

    public UpdateGroupCommandHandler(
        IGroupRepository groupRepository,
        ICurrentUserService currentUserService)
    {
        _groupRepository = groupRepository;
        _currentUserService = currentUserService;
    }

    public async ValueTask<GroupViewModel> Handle(
        UpdateGroupCommand request, 
        CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId 
            ?? throw new UnauthorizedAccessException("User not authenticated");

        var group = await _groupRepository.GetByIdWithMembersAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException($"Group with ID {request.Id} not found");

        // Only owner can update
        if (!group.IsOwner(userId))
            throw new ForbiddenException("Only the owner can update the group");

        group.SetName(request.Name);
        group.SetDescription(request.Description);

        await _groupRepository.UpdateAsync(group, cancellationToken);

        return new GroupViewModel(group, userId);
    }
}
