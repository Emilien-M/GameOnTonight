using FluentValidation;
using GameOnTonight.Application.BoardGames.ViewModels;
using GameOnTonight.Domain.Repositories;
using GameOnTonight.Domain.Services;
using Mediator;

namespace GameOnTonight.Application.BoardGames.Commands;

public sealed record UpdateBoardGameCommand(
    int Id,
    string Name,
    int MinPlayers,
    int MaxPlayers,
    int DurationMinutes,
    IReadOnlyList<string> GameTypes
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

        RuleFor(x => x.GameTypes)
            .NotNull();
        
        RuleForEach(x => x.GameTypes)
            .NotEmpty()
            .MaximumLength(100);
    }
}

public sealed class UpdateBoardGameCommandHandler : IRequestHandler<UpdateBoardGameCommand, BoardGameViewModel?>
{
    private readonly IBoardGameRepository _repository;
    private readonly IGameTypeRepository _gameTypeRepository;
    private readonly ICurrentUserService _currentUserService;

    public UpdateBoardGameCommandHandler(
        IBoardGameRepository repository, 
        IGameTypeRepository gameTypeRepository,
        ICurrentUserService currentUserService)
    {
        _repository = repository;
        _gameTypeRepository = gameTypeRepository;
        _currentUserService = currentUserService;
    }

    public async ValueTask<BoardGameViewModel?> Handle(UpdateBoardGameCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (entity is null)
        {
            return null;
        }

        entity.Update(
            request.Name,
            request.MinPlayers,
            request.MaxPlayers,
            request.DurationMinutes
        );

        // Update game types
        var gameTypes = request.GameTypes.Count > 0 
            ? await _gameTypeRepository.GetOrCreateByNamesAsync(request.GameTypes, cancellationToken)
            : [];
        entity.SetGameTypes(gameTypes);

        await _repository.UpdateAsync(entity, cancellationToken);

        return new BoardGameViewModel(entity, _currentUserService.UserId);
    }
}
