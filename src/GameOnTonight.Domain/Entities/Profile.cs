using GameOnTonight.Domain.Entities.Common;

namespace GameOnTonight.Domain.Entities;

public class Profile : UserOwnedEntity
{
    public string DisplayName { get; private set; } = string.Empty;

    private Profile() { }
    
    public Profile(string displayName)
    {
        DisplayName = displayName;
    }
    
    /// <summary>
    /// Updates the display name with validation.
    /// </summary>
    /// <param name="displayName">The new display name.</param>
    public void UpdateDisplayName(string displayName)
    {
        if (ValidateString(displayName, nameof(DisplayName), maxLength: 50))
        {
            DisplayName = displayName;
        }
    }
}
