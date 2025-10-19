using FluentValidation;
using GameOnTonight.Application.BoardGames.ViewModels;
using GameOnTonight.Domain.Repositories;
using Mediator;

namespace GameOnTonight.Application.BoardGames.Commands;

public sealed record UpdateBoardGameCommand(
    int Id,
    string Name,
    int MinPlayers,
    int MaxPlayers,
    int DurationMinutes,
    string GameType
) : IRequest<BoardGameViewModel?>;

public sealed class UpdateBoardGameCommandValidator : AbstractValidator<UpdateBoardGameCommand>
{
    public UpdateBoardGameCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0);

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.MinPlayers)
            .GreaterThanOrEqualTo(1);

        RuleFor(x => x.MaxPlayers)
            .GreaterThanOrEqualTo(x => x.MinPlayers);

        RuleFor(x => x.DurationMinutes)
            .GreaterThan(0);

        RuleFor(x => x.GameType)
            .NotEmpty()
            .MaximumLength(100);
    }
}

public sealed class UpdateBoardGameCommandHandler : IRequestHandler<UpdateBoardGameCommand, BoardGameViewModel?>
{
    private readonly IBoardGameRepository _repository;

    public UpdateBoardGameCommandHandler(IBoardGameRepository repository)
    {
        _repository = repository;
    }

    public async ValueTask<BoardGameViewModel?> Handle(UpdateBoardGameCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id);
        if (entity is null)
        {
            return null;
        }

        entity.Name = request.Name.Trim();
        entity.MinPlayers = request.MinPlayers;
        entity.MaxPlayers = request.MaxPlayers;
        entity.DurationMinutes = request.DurationMinutes;
        entity.GameType = request.GameType.Trim();

        await _repository.UpdateAsync(entity);

        return new BoardGameViewModel(entity);
    }
}
