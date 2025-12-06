using GameOnTonight.RestClient.Models;

namespace GameOnTonight.App.Services;

/// <summary>
/// Service pour la gestion des groupes.
/// </summary>
public interface IGroupsService
{
    #region Lecture
    
    /// <summary>
    /// Récupère tous les groupes dont l'utilisateur est membre.
    /// </summary>
    Task<IReadOnlyList<GroupViewModel>> GetMyGroupsAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Récupère les détails d'un groupe.
    /// </summary>
    Task<GroupDetailViewModel?> GetByIdAsync(int groupId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Récupère les membres d'un groupe.
    /// </summary>
    Task<IReadOnlyList<GroupMemberViewModel>> GetMembersAsync(int groupId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Récupère les codes d'invitation actifs d'un groupe.
    /// </summary>
    Task<IReadOnlyList<GroupInviteCodeViewModel>> GetInviteCodesAsync(int groupId, CancellationToken cancellationToken = default);
    
    #endregion
    
    #region Création/Modification
    
    /// <summary>
    /// Crée un nouveau groupe.
    /// </summary>
    Task<GroupDetailViewModel> CreateAsync(string name, string? description, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Met à jour un groupe existant.
    /// </summary>
    Task<GroupViewModel> UpdateAsync(int groupId, string name, string? description, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Supprime un groupe (Owner uniquement).
    /// </summary>
    Task DeleteAsync(int groupId, CancellationToken cancellationToken = default);
    
    #endregion
    
    #region Membres
    
    /// <summary>
    /// Rejoint un groupe avec un code d'invitation.
    /// </summary>
    Task<GroupViewModel> JoinAsync(string inviteCode, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Quitte un groupe.
    /// </summary>
    Task LeaveAsync(int groupId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Exclut un membre du groupe (Owner uniquement).
    /// </summary>
    Task RemoveMemberAsync(int groupId, string memberId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Transfère la propriété du groupe à un autre membre.
    /// </summary>
    Task TransferOwnershipAsync(int groupId, string newOwnerId, CancellationToken cancellationToken = default);
    
    #endregion
    
    #region Codes d'invitation
    
    /// <summary>
    /// Crée un nouveau code d'invitation (Owner uniquement).
    /// </summary>
    Task<GroupInviteCodeViewModel> CreateInviteCodeAsync(int groupId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Révoque un code d'invitation (Owner uniquement).
    /// </summary>
    Task RevokeInviteCodeAsync(int groupId, int codeId, CancellationToken cancellationToken = default);
    
    #endregion
}
