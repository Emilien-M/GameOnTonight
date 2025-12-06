using FluentValidation;
using GameOnTonight.Application.BoardGames.ViewModels;
using GameOnTonight.Domain.Repositories;
using GameOnTonight.Domain.Services;
using Mediator;

namespace GameOnTonight.Application.BoardGames.Queries;

public sealed record SuggestBoardGameQuery(int PlayersCount, int MaxDurationMinutes, IReadOnlyList<string>? GameTypes) : IRequest<BoardGameViewModel?>;

public sealed class SuggestBoardGameQueryValidator : AbstractValidator<SuggestBoardGameQuery>
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
        
        RuleForEach(x => x.GameTypes)
            .NotEmpty()
            .MaximumLength(100);
    }
}

public sealed class SuggestBoardGameQueryHandler : IRequestHandler<SuggestBoardGameQuery, BoardGameViewModel?>
{
    private readonly IBoardGameRepository _repository;
    private readonly ICurrentUserService _currentUserService;

    public SuggestBoardGameQueryHandler(IBoardGameRepository repository, ICurrentUserService currentUserService)
    {
        _repository = repository;
        _currentUserService = currentUserService;
    }

    public async ValueTask<BoardGameViewModel?> Handle(SuggestBoardGameQuery request, CancellationToken cancellationToken)
    {
        var trimmedGameTypes = request.GameTypes?
            .Select(t => t?.Trim())
            .Where(t => !string.IsNullOrEmpty(t))
            .Cast<string>()
            .ToList();
        
        var filtered = await _repository.FilterGamesAsync(
            request.PlayersCount, 
            request.MaxDurationMinutes, 
            trimmedGameTypes?.Count > 0 ? trimmedGameTypes : null, 
            cancellationToken);
        
        var ids = filtered.Select(g => g.Id).ToList();
        if (ids.Count == 0) return null;
        var chosen = await _repository.GetRandomGameAsync(ids, cancellationToken);
        return chosen is null ? null : new BoardGameViewModel(chosen, _currentUserService.UserId);
    }
}
