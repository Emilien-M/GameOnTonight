using FluentValidation;
using GameOnTonight.Application.BoardGames.ViewModels;
using GameOnTonight.Domain.Repositories;
using Mediator;

namespace GameOnTonight.Application.BoardGames.Queries;

public sealed record SuggestBoardGameQuery(int PlayersCount, int MaxDurationMinutes, string? GameType) : IRequest<BoardGameViewModel?>;

public class SuggestBoardGameQueryValidator : AbstractValidator<SuggestBoardGameQuery>
{
    public SuggestBoardGameQueryValidator()
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

public sealed class SuggestBoardGameQueryHandler : IRequestHandler<SuggestBoardGameQuery, BoardGameViewModel?>
{
    private readonly IBoardGameRepository _repository;

    public SuggestBoardGameQueryHandler(IBoardGameRepository repository)
    {
        _repository = repository;
    }

    public async ValueTask<BoardGameViewModel?> Handle(SuggestBoardGameQuery request, CancellationToken cancellationToken)
    {
        var filtered = await _repository.FilterGamesAsync(request.PlayersCount, request.MaxDurationMinutes, request.GameType?.Trim());
        var ids = filtered.Select(g => g.Id).ToList();
        if (ids.Count == 0) return null;
        var chosen = await _repository.GetRandomGameAsync(ids);
        return chosen is null ? null : new BoardGameViewModel(chosen);
    }
}
