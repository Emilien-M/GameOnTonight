using GameOnTonight.App.Components.Groups;
using GameOnTonight.RestClient.Models;

namespace GameOnTonight.App.Services;

/// <summary>
/// Service de gestion du contexte de filtrage par groupe.
/// </summary>
public interface IGroupContextService
{
    /// <summary>
    /// ID du groupe actuellement sélectionné (null si mode All ou PersonalOnly).
    /// </summary>
    int? SelectedGroupId { get; }
    
    /// <summary>
    /// Nom du groupe actuellement sélectionné.
    /// </summary>
    string? SelectedGroupName { get; }
    
    /// <summary>
    /// Mode de filtrage actuel.
    /// </summary>
    GroupFilterMode FilterMode { get; }
    
    /// <summary>
    /// Modifie le filtre de groupe.
    /// </summary>
    /// <param name="mode">Le mode de filtrage.</param>
    /// <param name="groupId">L'ID du groupe (requis si mode = GroupOnly).</param>
    /// <param name="groupName">Le nom du groupe (pour affichage).</param>
    void SetFilter(GroupFilterMode mode, int? groupId = null, string? groupName = null);
    
    /// <summary>
    /// Réinitialise le filtre à "Tous".
    /// </summary>
    void ResetFilter();
    
    /// <summary>
    /// Événement déclenché quand le filtre change.
    /// </summary>
    event Action? OnFilterChanged;
    
    /// <summary>
    /// Récupère la liste des groupes de l'utilisateur (avec cache).
    /// </summary>
    Task<IReadOnlyList<GroupViewModel>> GetUserGroupsAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Invalide le cache des groupes (à appeler après création/suppression/rejoindre/quitter).
    /// </summary>
    void InvalidateGroupsCache();
}
