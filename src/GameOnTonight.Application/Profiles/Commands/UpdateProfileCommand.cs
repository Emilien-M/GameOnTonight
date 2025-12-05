using FluentValidation;
using GameOnTonight.Application.Profiles.ViewModels;
using GameOnTonight.Domain.Entities;
using GameOnTonight.Domain.Repositories;
using Mediator;

namespace GameOnTonight.Application.Profiles.Commands;

/// <summary>
/// Command to update the user's profile display name.
/// </summary>
public sealed record UpdateProfileCommand(string DisplayName) : IRequest<ProfileViewModel>;

/// <summary>
/// Validator for UpdateProfileCommand.
/// </summary>
public sealed class UpdateProfileCommandValidator : AbstractValidator<UpdateProfileCommand>
{
    public UpdateProfileCommandValidator()
    {
        RuleFor(x => x.DisplayName)
            .NotEmpty().WithMessage("Le pseudo est requis")
            .MinimumLength(2).WithMessage("Le pseudo doit contenir au moins 2 caractères")
            .MaximumLength(50).WithMessage("Le pseudo ne peut pas dépasser 50 caractères");
    }
}

/// <summary>
/// Handler for UpdateProfileCommand.
/// </summary>
public sealed class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand, ProfileViewModel>
{
    private readonly IProfileRepository _profileRepository;
    private readonly IBoardGameRepository _boardGameRepository;
    private readonly IGameSessionRepository _sessionRepository;

    public UpdateProfileCommandHandler(
        IProfileRepository profileRepository,
        IBoardGameRepository boardGameRepository,
        IGameSessionRepository sessionRepository)
    {
        _profileRepository = profileRepository;
        _boardGameRepository = boardGameRepository;
        _sessionRepository = sessionRepository;
    }
    
    public async ValueTask<ProfileViewModel> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
    {
        var profile = await _profileRepository.GetAsync(cancellationToken);

        if (profile == null)
        {
            // Create profile if it doesn't exist
            profile = new Profile(request.DisplayName);
            await _profileRepository.AddAsync(profile, cancellationToken);
        }
        else
        {
            profile.UpdateDisplayName(request.DisplayName);
            await _profileRepository.UpdateAsync(profile, cancellationToken);
        }
        
        // Retrieve stats to return complete ViewModel
        var games = await _boardGameRepository.GetAllAsync(cancellationToken);
        var sessions = await _sessionRepository.GetSessionHistoryAsync();
        
        var gamesList = games.ToList();
        var sessionsList = sessions.ToList();
        
        var ratingsWithValue = sessionsList
            .Where(s => s.Rating.HasValue)
            .Select(s => s.Rating!.Value)
            .ToList();
        var averageRating = ratingsWithValue.Count > 0 
            ? Math.Round(ratingsWithValue.Average(), 1) 
            : (double?)null;

        var lastSession = sessionsList.FirstOrDefault();

        return new ProfileViewModel
        {
            DisplayName = profile.DisplayName,
            CreatedAt = profile.CreatedAt,
            MemberSince = GetMemberSince(profile.CreatedAt),
            AvatarInitials = GetInitials(profile.DisplayName),
            TotalGames = gamesList.Count,
            TotalSessions = sessionsList.Count,
            WinRate = null,
            AverageRating = averageRating,
            LastSession = lastSession != null ? new LastSessionViewModel
            {
                GameName = lastSession.BoardGame?.Name ?? "Jeu inconnu",
                PlayedAt = lastSession.PlayedAt,
                PlayerCount = lastSession.PlayerCount,
                Rating = lastSession.Rating,
                TimeAgo = GetTimeAgo(lastSession.PlayedAt)
            } : null
        };
    }
    
    private static string GetMemberSince(DateTime createdAt)
    {
        var diff = DateTime.UtcNow - createdAt;
        var totalDays = (int)diff.TotalDays;
        
        return totalDays switch
        {
            < 1 => "Membre depuis aujourd'hui",
            < 30 => $"Membre depuis {totalDays} jour{(totalDays > 1 ? "s" : "")}",
            < 365 => $"Membre depuis {totalDays / 30} mois",
            _ => $"Membre depuis {totalDays / 365} an{(totalDays / 365 > 1 ? "s" : "")}"
        };
    }
    
    private static string GetInitials(string displayName)
    {
        if (string.IsNullOrWhiteSpace(displayName))
            return "?";
            
        var parts = displayName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return parts.Length switch
        {
            0 => "?",
            1 => parts[0][..Math.Min(2, parts[0].Length)].ToUpperInvariant(),
            _ => $"{parts[0][0]}{parts[^1][0]}".ToUpperInvariant()
        };
    }
    
    private static string GetTimeAgo(DateTime date)
    {
        var diff = DateTime.UtcNow - date;
        var totalDays = (int)diff.TotalDays;
        
        return totalDays switch
        {
            < 1 => "Aujourd'hui",
            < 2 => "Hier",
            < 7 => $"Il y a {totalDays} jours",
            < 30 => $"Il y a {totalDays / 7} semaine{(totalDays / 7 > 1 ? "s" : "")}",
            < 365 => $"Il y a {totalDays / 30} mois",
            _ => $"Il y a {totalDays / 365} an{(totalDays / 365 > 1 ? "s" : "")}"
        };
    }
}
