using GameOnTonight.Application.BoardGames.ViewModels;
using GameOnTonight.Domain.Repositories;
using GameOnTonight.Domain.Services;
using Mediator;

namespace GameOnTonight.Application.BoardGames.Queries;

/// <summary>
/// Query pour récupérer un jeu de société par son ID
/// </summary>
public record GetBoardGameByIdQuery(int Id) : IRequest<BoardGameViewModel?>;

/// <summary>
/// Handler pour GetBoardGameByIdQuery
/// </summary>
public class GetBoardGameByIdQueryHandler : IRequestHandler<GetBoardGameByIdQuery, BoardGameViewModel?>
{
    private readonly IBoardGameRepository _boardGameRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetBoardGameByIdQueryHandler(
        IBoardGameRepository boardGameRepository,
        ICurrentUserService currentUserService)
    {
        _boardGameRepository = boardGameRepository;
        _currentUserService = currentUserService;
    }

    public async ValueTask<BoardGameViewModel?> Handle(GetBoardGameByIdQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId!;
        
        var boardGame = await _boardGameRepository.GetByIdAsync(request.Id, userId);
        if (boardGame == null)
        {
            return null;
        }

        return new BoardGameViewModel(boardGame);
    }
}
