using GameOnTonight.Application.BoardGames.ViewModels;
using GameOnTonight.Domain.Repositories;
using GameOnTonight.Domain.Services;
using Mediator;

namespace GameOnTonight.Application.BoardGames.Queries;

public sealed record GetAllBoardGamesQuery(int? GroupId = null) : IRequest<IEnumerable<BoardGameViewModel>>;

public sealed class GetAllBoardGamesQueryHandler : IRequestHandler<GetAllBoardGamesQuery, IEnumerable<BoardGameViewModel>>
{
    private readonly IBoardGameRepository _repository;
    private readonly ICurrentUserService _currentUserService;

    public GetAllBoardGamesQueryHandler(IBoardGameRepository repository, ICurrentUserService currentUserService)
    {
        _repository = repository;
        _currentUserService = currentUserService;
    }

    public async ValueTask<IEnumerable<BoardGameViewModel>> Handle(GetAllBoardGamesQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        var entities = await _repository.GetAllAsync(cancellationToken);
        
        // Filter by group if specified
        if (request.GroupId.HasValue)
        {
            entities = entities.Where(e => e.GroupId == request.GroupId.Value);
        }
        
        return entities.Select(e => new BoardGameViewModel(e, userId)).ToList();
    }
}
