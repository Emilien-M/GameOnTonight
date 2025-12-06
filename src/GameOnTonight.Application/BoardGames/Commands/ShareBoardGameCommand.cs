using FluentValidation;
using GameOnTonight.Application.BoardGames.ViewModels;
using GameOnTonight.Domain.Exceptions;
using GameOnTonight.Domain.Repositories;
using GameOnTonight.Domain.Services;
using Mediator;

namespace GameOnTonight.Application.BoardGames.Commands;

/// <summary>
/// Command to share a board game with a group.
/// </summary>
public sealed record ShareBoardGameCommand(int BoardGameId, int GroupId) : IRequest<BoardGameViewModel>;

public sealed class ShareBoardGameCommandValidator : AbstractValidator<ShareBoardGameCommand>
{
    public ShareBoardGameCommandValidator()
    {
        RuleFor(x => x.BoardGameId)
            .GreaterThan(0)
            .WithMessage("BoardGameId must be greater than 0");

        RuleFor(x => x.GroupId)
            .GreaterThan(0)
            .WithMessage("GroupId must be greater than 0");
    }
}

public sealed class ShareBoardGameCommandHandler : IRequestHandler<ShareBoardGameCommand, BoardGameViewModel>
{
    private readonly IBoardGameRepository _boardGameRepository;
    private readonly IGroupRepository _groupRepository;
    private readonly ICurrentUserService _currentUserService;

    public ShareBoardGameCommandHandler(
        IBoardGameRepository boardGameRepository,
        IGroupRepository groupRepository,
        ICurrentUserService currentUserService)
    {
        _boardGameRepository = boardGameRepository;
        _groupRepository = groupRepository;
        _currentUserService = currentUserService;
    }

    public async ValueTask<BoardGameViewModel> Handle(ShareBoardGameCommand request, CancellationToken cancellationToken)
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
            throw new ForbiddenException("You can only share your own board games");
        }

        // Verify the user is a member of the group
        var isMember = await _groupRepository.IsUserMemberAsync(request.GroupId, userId, cancellationToken);
        if (!isMember)
        {
            throw new ForbiddenException("You must be a member of the group to share a board game with it");
        }

        // Share the board game with the group
        boardGame.ShareWithGroup(request.GroupId);
        await _boardGameRepository.UpdateAsync(boardGame, cancellationToken);

        return new BoardGameViewModel(boardGame, userId);
    }
}
