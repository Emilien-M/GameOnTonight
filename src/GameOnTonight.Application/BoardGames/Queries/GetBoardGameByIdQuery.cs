using GameOnTonight.Application.BoardGames.ViewModels;
using GameOnTonight.Domain.Repositories;
using Mediator;

namespace GameOnTonight.Application.BoardGames.Queries;

public sealed record GetBoardGameByIdQuery(int Id) : IRequest<BoardGameViewModel?>;

public sealed class GetBoardGameByIdQueryHandler : IRequestHandler<GetBoardGameByIdQuery, BoardGameViewModel?>
{
    private readonly IBoardGameRepository _repository;

    public GetBoardGameByIdQueryHandler(IBoardGameRepository repository)
    {
        _repository = repository;
    }

    public async ValueTask<BoardGameViewModel?> Handle(GetBoardGameByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id);
        return entity is null ? null : new BoardGameViewModel(entity);
    }
}
