using GameOnTonight.Domain.Repositories;
using GameOnTonight.Domain.Services;
using Mediator;

namespace GameOnTonight.Application.BoardGames.Commands;

/// <summary>
/// Command to delete a board game.
/// </summary>
public record DeleteBoardGameCommand(int Id) : IRequest;

/// <summary>
/// Handler for DeleteBoardGameCommand.
/// </summary>
public class DeleteBoardGameCommandHandler : IRequestHandler<DeleteBoardGameCommand>
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

    public async ValueTask<Unit> Handle(DeleteBoardGameCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId!;
        
        var boardGame = await _boardGameRepository.GetByIdAsync(request.Id, userId);
        if (boardGame == null)
        {
            return Unit.Value;
        }

        var result = await _boardGameRepository.RemoveAsync(boardGame);
        await _unitOfWork.SaveChangesAsync();

        return Unit.Value;
    }
}