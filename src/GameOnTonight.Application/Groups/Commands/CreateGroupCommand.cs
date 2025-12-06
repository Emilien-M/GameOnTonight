using FluentValidation;
using GameOnTonight.Application.Groups.ViewModels;
using GameOnTonight.Domain.Entities;
using GameOnTonight.Domain.Repositories;
using GameOnTonight.Domain.Services;
using Mediator;

namespace GameOnTonight.Application.Groups.Commands;

/// <summary>
/// Command to create a new group.
/// </summary>
public sealed record CreateGroupCommand(
    string Name,
    string? Description) : IRequest<GroupDetailViewModel>;

public sealed class CreateGroupCommandValidator : AbstractValidator<CreateGroupCommand>
{
    public CreateGroupCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description must not exceed 500 characters");
    }
}

public sealed class CreateGroupCommandHandler : IRequestHandler<CreateGroupCommand, GroupDetailViewModel>
{
    private readonly IGroupRepository _groupRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly TimeProvider _timeProvider;

    public CreateGroupCommandHandler(
        IGroupRepository groupRepository,
        ICurrentUserService currentUserService, 
        TimeProvider timeProvider)
    {
        _groupRepository = groupRepository;
        _currentUserService = currentUserService;
        _timeProvider = timeProvider;
    }

    public async ValueTask<GroupDetailViewModel> Handle(
        CreateGroupCommand request, 
        CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId 
            ?? throw new UnauthorizedAccessException("User not authenticated");

        var group = new Group(request.Name, userId, _timeProvider.GetUtcNow().UtcDateTime, request.Description);
        
        await _groupRepository.AddAsync(group, cancellationToken);
        
        return new GroupDetailViewModel(group, userId, _timeProvider.GetUtcNow().UtcDateTime);
    }
}
