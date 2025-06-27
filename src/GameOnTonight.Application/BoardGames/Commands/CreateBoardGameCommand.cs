using GameOnTonight.Application.BoardGames.ViewModels;
using GameOnTonight.Domain.Entities;
using GameOnTonight.Domain.Repositories;
using Mediator;

namespace GameOnTonight.Application.BoardGames.Commands;

/// <summary>
/// Command to create a new board game.
/// </summary>
public record CreateBoardGameCommand(
    string Name, 
    int MinPlayers, 
    int MaxPlayers, 
    int DurationMinutes, 
    string GameType,
    string? Description = null,
    string? ImageUrl = null
) : IRequest<BoardGameViewModel>;

/// <summary>
/// Handler for CreateBoardGameCommand.
/// </summary>
public class CreateBoardGameCommandHandler : IRequestHandler<CreateBoardGameCommand, BoardGameViewModel>
{
    private readonly IBoardGameRepository _boardGameRepository;

    public CreateBoardGameCommandHandler(IBoardGameRepository boardGameRepository)
    {
        _boardGameRepository = boardGameRepository;
    }

    public async ValueTask<BoardGameViewModel> Handle(CreateBoardGameCommand request, CancellationToken cancellationToken)
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

        return new BoardGameViewModel(boardGame);
    }
}
