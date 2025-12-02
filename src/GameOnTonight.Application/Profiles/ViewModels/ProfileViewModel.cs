using GameOnTonight.Domain.Entities;

namespace GameOnTonight.Application.Profiles.ViewModels;

public record ProfileViewModel
{
    public string DisplayName { get; init; }
    
    public ProfileViewModel(Profile profile)
    {
        DisplayName = profile.DisplayName;
    }
}
