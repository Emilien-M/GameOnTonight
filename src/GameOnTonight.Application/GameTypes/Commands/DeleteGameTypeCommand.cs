using FluentValidation;
using GameOnTonight.Domain.Repositories;
using Mediator;

namespace GameOnTonight.Application.GameTypes.Commands;

public sealed record DeleteGameTypeCommand(int Id) : IRequest<bool>;

public sealed class DeleteGameTypeCommandValidator : AbstractValidator<DeleteGameTypeCommand>
{
    public DeleteGameTypeCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0);
    }
}

public sealed class DeleteGameTypeCommandHandler : IRequestHandler<DeleteGameTypeCommand, bool>
{
    private readonly IGameTypeRepository _repository;

    public DeleteGameTypeCommandHandler(IGameTypeRepository repository)
    {
        _repository = repository;
    }

    public async ValueTask<bool> Handle(DeleteGameTypeCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (entity is null)
        {
            return false;
        }

        await _repository.RemoveAsync(entity, cancellationToken);
        return true;
    }
}
