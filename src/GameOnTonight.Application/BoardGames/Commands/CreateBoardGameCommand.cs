using GameOnTonight.Domain.Entities;
using GameOnTonight.Domain.Repositories;
using Mediator;

namespace GameOnTonight.Application.BoardGames.Commands;

/// <summary>
/// Command pour créer un nouveau jeu de société
/// </summary>
public record CreateBoardGameCommand(
    string Name, 
    int MinPlayers, 
    int MaxPlayers, 
    int DurationMinutes, 
    string GameType,
    string? Description = null,
    string? ImageUrl = null
) : IRequest<int>;

/// <summary>
/// Handler pour CreateBoardGameCommand
/// </summary>
public class CreateBoardGameCommandHandler : IRequestHandler<CreateBoardGameCommand, int>
{
    private readonly IBoardGameRepository _boardGameRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateBoardGameCommandHandler(
        IBoardGameRepository boardGameRepository,
        IUnitOfWork unitOfWork)
    {
        _boardGameRepository = boardGameRepository;
        _unitOfWork = unitOfWork;
    }

    public async ValueTask<int> Handle(CreateBoardGameCommand request, CancellationToken cancellationToken)
    {
        var boardGame = new BoardGame
        {
            Name = request.Name,
            MinPlayers = request.MinPlayers,
            MaxPlayers = request.MaxPlayers,
            DurationMinutes = request.DurationMinutes,
            GameType = request.GameType,
            Description = request.Description,
            ImageUrl = request.ImageUrl
        };

        await _boardGameRepository.AddAsync(boardGame);
        await _unitOfWork.SaveChangesAsync();

        return boardGame.Id;
    }
}
