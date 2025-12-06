using GameOnTonight.Application.BoardGames.ViewModels;
using GameOnTonight.Domain.Repositories;
using GameOnTonight.Domain.Services;
using Mediator;

namespace GameOnTonight.Application.BoardGames.Queries;

public sealed record GetBoardGameByIdQuery(int Id) : IRequest<BoardGameViewModel?>;

public sealed class GetBoardGameByIdQueryHandler : IRequestHandler<GetBoardGameByIdQuery, BoardGameViewModel?>
{
    private readonly IBoardGameRepository _repository;
    private readonly ICurrentUserService _currentUserService;

    public GetBoardGameByIdQueryHandler(IBoardGameRepository repository, ICurrentUserService currentUserService)
    {
        _repository = repository;
        _currentUserService = currentUserService;
    }

    public async ValueTask<BoardGameViewModel?> Handle(GetBoardGameByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken);
        return entity is null ? null : new BoardGameViewModel(entity, _currentUserService.UserId);
    }
}
