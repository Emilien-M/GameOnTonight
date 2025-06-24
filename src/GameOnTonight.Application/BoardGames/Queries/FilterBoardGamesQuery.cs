using GameOnTonight.Application.BoardGames.ViewModels;
using GameOnTonight.Domain.Repositories;
using GameOnTonight.Domain.Services;
using Mediator;

namespace GameOnTonight.Application.BoardGames.Queries;

/// <summary>
/// Query to filter games based on criteria (player count, duration, type).
/// </summary>
public record FilterBoardGamesQuery(
    int PlayerCount,
    int MaxDuration,
    string? GameType
) : IRequest<IEnumerable<BoardGameViewModel>>;

/// <summary>
/// Handler for FilterBoardGamesQuery.
/// </summary>
public class FilterBoardGamesQueryHandler : IRequestHandler<FilterBoardGamesQuery, IEnumerable<BoardGameViewModel>>
{
    private readonly IBoardGameRepository _boardGameRepository;
    private readonly ICurrentUserService _currentUserService;

    public FilterBoardGamesQueryHandler(
        IBoardGameRepository boardGameRepository,
        ICurrentUserService currentUserService)
    {
        _boardGameRepository = boardGameRepository;
        _currentUserService = currentUserService;
    }

    public async ValueTask<IEnumerable<BoardGameViewModel>> Handle(FilterBoardGamesQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId!;
        
        var boardGames = await _boardGameRepository.FilterGamesAsync(
            request.PlayerCount,
            request.MaxDuration,
            request.GameType,
            userId
        );
        
        return boardGames.Select(bg => new BoardGameViewModel(bg));
    }
}