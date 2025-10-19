using FluentValidation;
using GameOnTonight.Domain.Entities;
using GameOnTonight.Domain.Repositories;
using GameOnTonight.Domain.Services;
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
    private readonly ICurrentUserService _currentUserService;

    public DeleteBoardGameCommandHandler(IBoardGameRepository repository, ICurrentUserService currentUserService)
    {
        _repository = repository;
        _currentUserService = currentUserService;
    }

    public async ValueTask<bool> Handle(DeleteBoardGameCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId!;
        var entity = await _repository.GetByIdAsync(request.Id, userId);
        if (entity is null)
        {
            return false;
        }

        return await _repository.RemoveAsync(entity);
    }
}
