using FluentValidation;
using GameOnTonight.Domain.Repositories;
using Mediator;

namespace GameOnTonight.Application.BoardGames.Commands;

public sealed record DeleteBoardGameCommand(int Id) : IRequest<bool>;

public sealed class DeleteBoardGameCommandValidator : AbstractValidator<DeleteBoardGameCommand>
{
    public DeleteBoardGameCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0);
    }
}

public sealed class DeleteBoardGameCommandHandler : IRequestHandler<DeleteBoardGameCommand, bool>
{
    private readonly IBoardGameRepository _repository;

    public DeleteBoardGameCommandHandler(IBoardGameRepository repository)
    {
        _repository = repository;
    }

    public async ValueTask<bool> Handle(DeleteBoardGameCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id);
        if (entity is null)
        {
            return false;
        }

        return await _repository.RemoveAsync(entity);
    }
}
