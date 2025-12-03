using FluentValidation;
using GameOnTonight.Application.BoardGames.ViewModels;
using GameOnTonight.Domain.Entities;
using GameOnTonight.Domain.Repositories;
using Mediator;

namespace GameOnTonight.Application.BoardGames.Commands;

public sealed record CreateBoardGameCommand(
    string Name,
    int MinPlayers,
    int MaxPlayers,
    int DurationMinutes,
    IReadOnlyList<string> GameTypes
) : IRequest<BoardGameViewModel>;

public sealed class CreateBoardGameCommandValidator : AbstractValidator<CreateBoardGameCommand>
{
    public CreateBoardGameCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.MinPlayers)
            .GreaterThanOrEqualTo(1);

        RuleFor(x => x.MaxPlayers)
            .GreaterThanOrEqualTo(x => x.MinPlayers);

        RuleFor(x => x.DurationMinutes)
            .GreaterThan(0);

        RuleFor(x => x.GameTypes)
            .NotNull();
        
        RuleForEach(x => x.GameTypes)
            .NotEmpty()
            .MaximumLength(100);
    }
}

public sealed class CreateBoardGameCommandHandler : IRequestHandler<CreateBoardGameCommand, BoardGameViewModel>
{
    private readonly IBoardGameRepository _repository;
    private readonly IGameTypeRepository _gameTypeRepository;

    public CreateBoardGameCommandHandler(IBoardGameRepository repository, IGameTypeRepository gameTypeRepository)
    {
        _repository = repository;
        _gameTypeRepository = gameTypeRepository;
    }

    public async ValueTask<BoardGameViewModel> Handle(CreateBoardGameCommand request, CancellationToken cancellationToken)
    {
        var entity = new BoardGame(
            request.Name,
            request.MinPlayers,
            request.MaxPlayers,
            request.DurationMinutes
        );

        // Get or create game types
        if (request.GameTypes.Count > 0)
        {
            var gameTypes = await _gameTypeRepository.GetOrCreateByNamesAsync(request.GameTypes, cancellationToken);
            entity.SetGameTypes(gameTypes);
        }

        await _repository.AddAsync(entity, cancellationToken);

        return new BoardGameViewModel(entity);
    }
}
