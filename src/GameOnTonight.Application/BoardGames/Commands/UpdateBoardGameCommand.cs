using GameOnTonight.Application.BoardGames.ViewModels;
using GameOnTonight.Domain.Entities;
using GameOnTonight.Domain.Repositories;
using GameOnTonight.Domain.Services;
using Mediator;

namespace GameOnTonight.Application.BoardGames.Commands;

/// <summary>
/// Command to update an existing board game.
/// </summary>
public record UpdateBoardGameCommand(
    int Id,
    string Name, 
    int MinPlayers, 
    int MaxPlayers, 
    int DurationMinutes, 
    string GameType,
    string? Description = null,
    string? ImageUrl = null
) : IRequest<BoardGameViewModel>;

/// <summary>
/// Handler for UpdateBoardGameCommand.
/// </summary>
public class UpdateBoardGameCommandHandler : IRequestHandler<UpdateBoardGameCommand, BoardGameViewModel>
{
    private readonly IBoardGameRepository _boardGameRepository;
    private readonly ICurrentUserService _currentUserService;

    public UpdateBoardGameCommandHandler(IBoardGameRepository boardGameRepository, ICurrentUserService currentUserService)
    {
        _boardGameRepository = boardGameRepository;
        _currentUserService = currentUserService;
    }

    public async ValueTask<BoardGameViewModel> Handle(UpdateBoardGameCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId!;
        
        var boardGame = await _boardGameRepository.GetByIdAsync(request.Id, userId);
        if (boardGame == null)
        {
            throw BoardGameErrors.BoardGameNotFound(request.Id).Exception();
        }

        boardGame.Name = request.Name;
        boardGame.MinPlayers = request.MinPlayers;
        boardGame.MaxPlayers = request.MaxPlayers;
        boardGame.DurationMinutes = request.DurationMinutes;
        boardGame.GameType = request.GameType;
        boardGame.Description = request.Description;
        boardGame.ImageUrl = request.ImageUrl;

        await _boardGameRepository.UpdateAsync(boardGame);

        return new BoardGameViewModel(boardGame);
    }
}
