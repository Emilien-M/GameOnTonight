using GameOnTonight.Domain.Entities.Common;

namespace GameOnTonight.Domain.Entities;

public class Profile : UserOwnedEntity
{
    public string DisplayName { get; private set; }

    private Profile() { }
    
    public Profile(string displayName)
    {
        DisplayName = displayName;
    }
}
