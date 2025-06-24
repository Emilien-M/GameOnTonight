using GameOnTonight.Application.BoardGames.ViewModels;
using GameOnTonight.Domain.Repositories;
using GameOnTonight.Domain.Services;
using Mediator;

namespace GameOnTonight.Application.BoardGames.Queries;

/// <summary>
/// Query to select a random game from a list of IDs.
/// </summary>
public record GetRandomBoardGameQuery(IEnumerable<int> GameIds) : IRequest<BoardGameViewModel?>;

/// <summary>
/// Handler for GetRandomBoardGameQuery.
/// </summary>
public class GetRandomBoardGameQueryHandler : IRequestHandler<GetRandomBoardGameQuery, BoardGameViewModel?>
{
    private readonly IBoardGameRepository _boardGameRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IGameSessionRepository _gameSessionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public GetRandomBoardGameQueryHandler(
        IBoardGameRepository boardGameRepository,
        ICurrentUserService currentUserService,
        IGameSessionRepository gameSessionRepository,
        IUnitOfWork unitOfWork)
    {
        _boardGameRepository = boardGameRepository;
        _currentUserService = currentUserService;
        _gameSessionRepository = gameSessionRepository;
        _unitOfWork = unitOfWork;
    }

    public async ValueTask<BoardGameViewModel?> Handle(GetRandomBoardGameQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId!;
        
        var boardGame = await _boardGameRepository.GetRandomGameAsync(request.GameIds, userId);
        if (boardGame == null)
        {
            return null;
        }
        
        var gameSession = new GameOnTonight.Domain.Entities.GameSession
        {
            BoardGameId = boardGame.Id,
            PlayedAt = DateTime.UtcNow,
            PlayerCount = 0, 
            Notes = "Selected by the assistant"
        };
        
        await _gameSessionRepository.AddAsync(gameSession);
        await _unitOfWork.SaveChangesAsync();

        return new BoardGameViewModel(boardGame);
    }
}