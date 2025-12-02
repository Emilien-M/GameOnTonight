using FluentValidation;
using GameOnTonight.Application.GameSessions.ViewModels;
using GameOnTonight.Domain.Entities;
using GameOnTonight.Domain.Repositories;
using Mediator;

namespace GameOnTonight.Application.GameSessions.Commands;

/// <summary>
/// DTO for creating/updating a player in a game session.
/// </summary>
public sealed record PlayerInputDto(
    string PlayerName,
    int? Score = null,
    bool IsWinner = false,
    int? Position = null
);

public sealed record CreateGameSessionCommand(
    int BoardGameId,
    DateTime PlayedAt,
    int PlayerCount,
    string? Notes = null,
    int? Rating = null,
    string? PhotoUrl = null,
    List<PlayerInputDto>? Players = null
) : IRequest<GameSessionViewModel>;

public sealed class CreateGameSessionCommandValidator : AbstractValidator<CreateGameSessionCommand>
{
    public CreateGameSessionCommandValidator()
    {
        RuleFor(x => x.BoardGameId)
            .GreaterThan(0)
            .WithMessage("Le jeu de société est requis.");

        RuleFor(x => x.PlayedAt)
            .NotEmpty()
            .WithMessage("La date de la partie est requise.")
            .LessThanOrEqualTo(DateTime.UtcNow.AddDays(1))
            .WithMessage("La date de la partie ne peut pas être dans le futur.");

        RuleFor(x => x.PlayerCount)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Le nombre de joueurs doit être au moins 1.");

        RuleFor(x => x.Notes)
            .MaximumLength(500)
            .WithMessage("Les notes ne peuvent pas dépasser 500 caractères.");

        RuleFor(x => x.Rating)
            .InclusiveBetween(1, 5)
            .When(x => x.Rating.HasValue)
            .WithMessage("La note doit être comprise entre 1 et 5.");

        RuleFor(x => x.PhotoUrl)
            .MaximumLength(500)
            .When(x => !string.IsNullOrEmpty(x.PhotoUrl))
            .WithMessage("L'URL de la photo ne peut pas dépasser 500 caractères.");

        RuleForEach(x => x.Players)
            .ChildRules(player =>
            {
                player.RuleFor(p => p.PlayerName)
                    .NotEmpty()
                    .WithMessage("Le nom du joueur est requis.")
                    .MaximumLength(100)
                    .WithMessage("Le nom du joueur ne peut pas dépasser 100 caractères.");

                player.RuleFor(p => p.Score)
                    .GreaterThanOrEqualTo(0)
                    .When(p => p.Score.HasValue)
                    .WithMessage("Le score ne peut pas être négatif.");

                player.RuleFor(p => p.Position)
                    .GreaterThanOrEqualTo(1)
                    .When(p => p.Position.HasValue)
                    .WithMessage("La position doit être au moins 1.");
            });
    }
}

public sealed class CreateGameSessionCommandHandler : IRequestHandler<CreateGameSessionCommand, GameSessionViewModel>
{
    private readonly IGameSessionRepository _sessionRepository;
    private readonly IBoardGameRepository _boardGameRepository;

    public CreateGameSessionCommandHandler(
        IGameSessionRepository sessionRepository, 
        IBoardGameRepository boardGameRepository)
    {
        _sessionRepository = sessionRepository;
        _boardGameRepository = boardGameRepository;
    }

    public async ValueTask<GameSessionViewModel> Handle(CreateGameSessionCommand request, CancellationToken cancellationToken)
    {
        // Vérifier que le jeu existe et appartient à l'utilisateur
        var boardGame = await _boardGameRepository.GetByIdAsync(request.BoardGameId, cancellationToken);
        if (boardGame is null)
        {
            throw new InvalidOperationException($"Le jeu avec l'ID {request.BoardGameId} n'existe pas.");
        }

        // Ensure PlayedAt is in UTC for PostgreSQL compatibility
        var playedAtUtc = request.PlayedAt.Kind == DateTimeKind.Utc 
            ? request.PlayedAt 
            : DateTime.SpecifyKind(request.PlayedAt, DateTimeKind.Utc);

        var entity = new GameSession
        {
            BoardGameId = request.BoardGameId,
            BoardGame = boardGame,
            PlayedAt = playedAtUtc,
            PlayerCount = request.PlayerCount,
            Notes = request.Notes,
            PhotoUrl = request.PhotoUrl
        };

        // Set rating if provided
        if (request.Rating.HasValue)
        {
            entity.SetRating(request.Rating.Value);
        }

        // Add players if provided
        if (request.Players is { Count: > 0 })
        {
            foreach (var playerDto in request.Players)
            {
                var player = new GameSessionPlayer(
                    playerDto.PlayerName,
                    playerDto.Score,
                    playerDto.IsWinner,
                    playerDto.Position
                );
                entity.AddPlayer(player);
            }
        }

        await _sessionRepository.AddAsync(entity, cancellationToken);

        return new GameSessionViewModel(entity);
    }
}
