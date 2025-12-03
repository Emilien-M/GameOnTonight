using FluentValidation;
using GameOnTonight.Application.GameTypes.ViewModels;
using GameOnTonight.Domain.Entities;
using GameOnTonight.Domain.Repositories;
using Mediator;

namespace GameOnTonight.Application.GameTypes.Commands;

public sealed record CreateGameTypeCommand(string Name) : IRequest<GameTypeViewModel>;

public sealed class CreateGameTypeCommandValidator : AbstractValidator<CreateGameTypeCommand>
{
    public CreateGameTypeCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);
    }
}

public sealed class CreateGameTypeCommandHandler : IRequestHandler<CreateGameTypeCommand, GameTypeViewModel>
{
    private readonly IGameTypeRepository _repository;

    public CreateGameTypeCommandHandler(IGameTypeRepository repository)
    {
        _repository = repository;
    }

    public async ValueTask<GameTypeViewModel> Handle(CreateGameTypeCommand request, CancellationToken cancellationToken)
    {
        // Check if game type already exists
        var existing = await _repository.GetByNameAsync(request.Name, cancellationToken);
        if (existing != null)
        {
            return new GameTypeViewModel(existing);
        }

        var entity = new GameType(request.Name);
        await _repository.AddAsync(entity, cancellationToken);

        return new GameTypeViewModel(entity);
    }
}
