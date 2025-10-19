using FluentValidation;
using GameOnTonight.Application.BoardGames.ViewModels;
using GameOnTonight.Domain.Repositories;
using Mediator;

namespace GameOnTonight.Application.BoardGames.Queries;

public sealed record FilterBoardGamesQuery(int PlayersCount, int MaxDurationMinutes, string? GameType) : IRequest<IEnumerable<BoardGameViewModel>>;

public class FilterBoardGamesQueryValidator : AbstractValidator<FilterBoardGamesQuery>
{
    public FilterBoardGamesQueryValidator()
    {
        RuleFor(x => x.PlayersCount)
            .GreaterThanOrEqualTo(1)
            .WithName("PlayersCount")
            .WithMessage("PlayersCount must be greater than or equal to 1");
        
        RuleFor(x => x.MaxDurationMinutes)
            .GreaterThanOrEqualTo(1)
            .WithName("MaxDurationMinutes")
            .WithMessage("MaxDurationMinutes must be greater than or equal to 1");
    }
}

public sealed class FilterBoardGamesQueryHandler : IRequestHandler<FilterBoardGamesQuery, IEnumerable<BoardGameViewModel>>
{
    private readonly IBoardGameRepository _repository;

    public FilterBoardGamesQueryHandler(IBoardGameRepository repository)
    {
        _repository = repository;
    }

    public async ValueTask<IEnumerable<BoardGameViewModel>> Handle(FilterBoardGamesQuery request, CancellationToken cancellationToken)
    {
        var entities = await _repository.FilterGamesAsync(request.PlayersCount, request.MaxDurationMinutes, request.GameType?.Trim());
        return entities.Select(e => new BoardGameViewModel(e)).ToList();
    }
}
