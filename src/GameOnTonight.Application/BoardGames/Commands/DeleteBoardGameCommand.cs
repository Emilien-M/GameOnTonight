using GameOnTonight.Domain.Repositories;
using GameOnTonight.Domain.Services;
using Mediator;

namespace GameOnTonight.Application.BoardGames.Commands;

/// <summary>
/// Command pour supprimer un jeu de société
/// </summary>
public record DeleteBoardGameCommand(int Id) : IRequest<bool>;

/// <summary>
/// Handler pour DeleteBoardGameCommand
/// </summary>
public class DeleteBoardGameCommandHandler : IRequestHandler<DeleteBoardGameCommand, bool>
{
    private readonly IBoardGameRepository _boardGameRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteBoardGameCommandHandler(
        IBoardGameRepository boardGameRepository,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork)
    {
        _boardGameRepository = boardGameRepository;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }

    public async ValueTask<bool> Handle(DeleteBoardGameCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId!;
        
        var boardGame = await _boardGameRepository.GetByIdAsync(request.Id, userId);
        if (boardGame == null)
        {
            return false;
        }

        var result = await _boardGameRepository.RemoveAsync(boardGame);
        await _unitOfWork.SaveChangesAsync();

        return result;
    }
}