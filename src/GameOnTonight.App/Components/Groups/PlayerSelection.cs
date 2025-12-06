namespace GameOnTonight.App.Components.Groups;

/// <summary>
/// Représente un joueur sélectionné (membre du groupe ou invité).
/// </summary>
public class PlayerSelection
{
    /// <summary>
    /// Nom affiché du joueur.
    /// </summary>
    public string? DisplayName { get; set; }
    
    /// <summary>
    /// ID du membre du groupe (si lié à un membre).
    /// </summary>
    public int? GroupMemberId { get; set; }
    
    /// <summary>
    /// ID utilisateur (si lié à un membre).
    /// </summary>
    public string? UserId { get; set; }
    
    /// <summary>
    /// Indique si le joueur est un membre du groupe.
    /// </summary>
    public bool IsGroupMember { get; set; }
    
    /// <summary>
    /// Indique si c'est l'utilisateur courant.
    /// </summary>
    public bool IsCurrentUser { get; set; }
    
    /// <summary>
    /// Initiales pour l'avatar.
    /// </summary>
    public string Initials => GetInitials(DisplayName);
    
    private static string GetInitials(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return "?";
        
        var parts = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return parts.Length switch
        {
            0 => "?",
            1 => parts[0][..Math.Min(2, parts[0].Length)].ToUpper(),
            _ => $"{parts[0][0]}{parts[^1][0]}".ToUpper()
        };
    }
}
