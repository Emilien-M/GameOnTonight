using GameOnTonight.Domain.Repositories;
using GameOnTonight.Domain.Services;
using Mediator;

namespace GameOnTonight.Application.BoardGames.Commands;

/// <summary>
/// Command pour mettre à jour un jeu de société existant
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
) : IRequest<bool>;

/// <summary>
/// Handler pour UpdateBoardGameCommand
/// </summary>
public class UpdateBoardGameCommandHandler : IRequestHandler<UpdateBoardGameCommand, bool>
{
    private readonly IBoardGameRepository _boardGameRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateBoardGameCommandHandler(
        IBoardGameRepository boardGameRepository,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork)
    {
        _boardGameRepository = boardGameRepository;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }

    public async ValueTask<bool> Handle(UpdateBoardGameCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId!;
        
        var boardGame = await _boardGameRepository.GetByIdAsync(request.Id, userId);
        if (boardGame == null)
        {
            return false;
        }

        // Mise à jour des propriétés
        boardGame.Name = request.Name;
        boardGame.MinPlayers = request.MinPlayers;
        boardGame.MaxPlayers = request.MaxPlayers;
        boardGame.DurationMinutes = request.DurationMinutes;
        boardGame.GameType = request.GameType;
        boardGame.Description = request.Description;
        boardGame.ImageUrl = request.ImageUrl;

        var result = await _boardGameRepository.UpdateAsync(boardGame);
        await _unitOfWork.SaveChangesAsync();

        return result;
    }
}
