namespace GameOnTonight.App.Components.Groups;

/// <summary>
/// Option de filtrage pour le sélecteur de groupe.
/// </summary>
public record GroupFilterOption(GroupFilterMode Mode, int? GroupId, string? GroupName)
{
    public override string ToString() => GroupName ?? Mode.ToString();
}

/// <summary>
/// Mode de filtrage des données par groupe.
/// </summary>
public enum GroupFilterMode
{
    /// <summary>
    /// Afficher toutes les données (personnelles + partagées).
    /// </summary>
    All,
    
    /// <summary>
    /// Afficher uniquement les données personnelles.
    /// </summary>
    PersonalOnly,
    
    /// <summary>
    /// Afficher uniquement les données d'un groupe spécifique.
    /// </summary>
    GroupOnly
}
