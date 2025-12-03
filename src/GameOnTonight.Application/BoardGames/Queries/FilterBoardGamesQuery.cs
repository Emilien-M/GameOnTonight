using FluentValidation;
using GameOnTonight.Application.BoardGames.ViewModels;
using GameOnTonight.Domain.Repositories;
using Mediator;

namespace GameOnTonight.Application.BoardGames.Queries;

public sealed record FilterBoardGamesQuery(int PlayersCount, int MaxDurationMinutes, IReadOnlyList<string>? GameTypes) : IRequest<IEnumerable<BoardGameViewModel>>;

public sealed class FilterBoardGamesQueryValidator : AbstractValidator<FilterBoardGamesQuery>
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
        
        RuleForEach(x => x.GameTypes)
            .NotEmpty()
            .MaximumLength(100);
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
        var trimmedGameTypes = request.GameTypes?
            .Select(t => t?.Trim())
            .Where(t => !string.IsNullOrEmpty(t))
            .Cast<string>()
            .ToList();
        
        var entities = await _repository.FilterGamesAsync(
            request.PlayersCount, 
            request.MaxDurationMinutes, 
            trimmedGameTypes?.Count > 0 ? trimmedGameTypes : null, 
            cancellationToken);
        
        return entities.Select(e => new BoardGameViewModel(e)).ToList();
    }
}
