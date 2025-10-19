using GameOnTonight.Application.BoardGames.ViewModels;
using GameOnTonight.Domain.Repositories;
using Mediator;

namespace GameOnTonight.Application.BoardGames.Queries;

public sealed record GetAllBoardGamesQuery : IRequest<IEnumerable<BoardGameViewModel>>;

public sealed class GetAllBoardGamesQueryHandler : IRequestHandler<GetAllBoardGamesQuery, IEnumerable<BoardGameViewModel>>
{
    private readonly IBoardGameRepository _repository;

    public GetAllBoardGamesQueryHandler(IBoardGameRepository repository)
    {
        _repository = repository;
    }

    public async ValueTask<IEnumerable<BoardGameViewModel>> Handle(GetAllBoardGamesQuery request, CancellationToken cancellationToken)
    {
        var entities = await _repository.GetAllAsync();
        return entities.Select(e => new BoardGameViewModel(e)).ToList();
    }
}
