using GameOnTonight.Application.BoardGames.ViewModels;
using GameOnTonight.Domain.Repositories;
using GameOnTonight.Domain.Services;
using Mediator;

namespace GameOnTonight.Application.BoardGames.Queries;

/// <summary>
/// Query to retrieve all of the user's board games.
/// </summary>
public record GetAllBoardGamesQuery : IRequest<IEnumerable<BoardGameViewModel>>;

/// <summary>
/// Handler for GetAllBoardGamesQuery.
/// </summary>
public class GetAllBoardGamesQueryHandler : IRequestHandler<GetAllBoardGamesQuery, IEnumerable<BoardGameViewModel>>
{
    private readonly IBoardGameRepository _boardGameRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetAllBoardGamesQueryHandler(
        IBoardGameRepository boardGameRepository,
        ICurrentUserService currentUserService)
    {
        _boardGameRepository = boardGameRepository;
        _currentUserService = currentUserService;
    }

    public async ValueTask<IEnumerable<BoardGameViewModel>> Handle(GetAllBoardGamesQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId!;
        
        var boardGames = await _boardGameRepository.GetAllAsync(userId);
        
        return boardGames.Select(bg => new BoardGameViewModel(bg));
    }
}