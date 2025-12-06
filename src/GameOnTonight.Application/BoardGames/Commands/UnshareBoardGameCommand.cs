using FluentValidation;
using GameOnTonight.Application.BoardGames.ViewModels;
using GameOnTonight.Domain.Exceptions;
using GameOnTonight.Domain.Repositories;
using GameOnTonight.Domain.Services;
using Mediator;

namespace GameOnTonight.Application.BoardGames.Commands;

/// <summary>
/// Command to unshare a board game from a group (make it private again).
/// </summary>
public sealed record UnshareBoardGameCommand(int BoardGameId) : IRequest<BoardGameViewModel>;

public sealed class UnshareBoardGameCommandValidator : AbstractValidator<UnshareBoardGameCommand>
{
    public UnshareBoardGameCommandValidator()
    {
        RuleFor(x => x.BoardGameId)
            .GreaterThan(0)
            .WithMessage("BoardGameId must be greater than 0");
    }
}

public sealed class UnshareBoardGameCommandHandler : IRequestHandler<UnshareBoardGameCommand, BoardGameViewModel>
{
    private readonly IBoardGameRepository _boardGameRepository;
    private readonly ICurrentUserService _currentUserService;

    public UnshareBoardGameCommandHandler(
        IBoardGameRepository boardGameRepository,
        ICurrentUserService currentUserService)
    {
        _boardGameRepository = boardGameRepository;
        _currentUserService = currentUserService;
    }

    public async ValueTask<BoardGameViewModel> Handle(UnshareBoardGameCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId
            ?? throw new UnauthorizedAccessException("User not authenticated");

        // Get the board game
        var boardGame = await _boardGameRepository.GetByIdAsync(request.BoardGameId, cancellationToken);
        if (boardGame is null)
        {
            throw new NotFoundException("BoardGame", request.BoardGameId);
        }

        // Verify the user owns the board game
        if (boardGame.UserId != userId)
        {
            throw new ForbiddenException("You can only unshare your own board games");
        }

        // Make the board game private
        boardGame.MakePrivate();
        await _boardGameRepository.UpdateAsync(boardGame, cancellationToken);

        return new BoardGameViewModel(boardGame, userId);
    }
}
