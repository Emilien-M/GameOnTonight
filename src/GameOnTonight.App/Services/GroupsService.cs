using System.Collections.ObjectModel;
using GameOnTonight.RestClient.Models;

namespace GameOnTonight.App.Services;

/// <summary>
/// Implémentation du service de gestion des groupes.
/// </summary>
public class GroupsService : IGroupsService
{
    private readonly GameOnTonightClientFactory _clientFactory;

    public GroupsService(GameOnTonightClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }

    #region Lecture

    public async Task<IReadOnlyList<GroupViewModel>> GetMyGroupsAsync(CancellationToken cancellationToken = default)
    {
        var client = _clientFactory.CreateClient();
        var result = await client.Groups.GetAsync(cancellationToken: cancellationToken);
        return result?.AsReadOnly() ?? ReadOnlyCollection<GroupViewModel>.Empty;
    }

    public async Task<GroupDetailViewModel?> GetByIdAsync(int groupId, CancellationToken cancellationToken = default)
    {
        var client = _clientFactory.CreateClient();
        return await client.Groups[groupId].GetAsync(cancellationToken: cancellationToken);
    }

    public async Task<IReadOnlyList<GroupMemberViewModel>> GetMembersAsync(int groupId, CancellationToken cancellationToken = default)
    {
        var client = _clientFactory.CreateClient();
        var result = await client.Groups[groupId].Members.GetAsync(cancellationToken: cancellationToken);
        return result?.AsReadOnly() ?? ReadOnlyCollection<GroupMemberViewModel>.Empty;
    }

    public async Task<IReadOnlyList<GroupInviteCodeViewModel>> GetInviteCodesAsync(int groupId, CancellationToken cancellationToken = default)
    {
        var client = _clientFactory.CreateClient();
        var result = await client.Groups[groupId].InviteCodes.GetAsync(cancellationToken: cancellationToken);
        return result?.AsReadOnly() ?? ReadOnlyCollection<GroupInviteCodeViewModel>.Empty;
    }

    #endregion

    #region Création/Modification

    public async Task<GroupDetailViewModel> CreateAsync(string name, string? description, CancellationToken cancellationToken = default)
    {
        var client = _clientFactory.CreateClient();
        var request = new CreateGroupCommand
        {
            Name = name,
            Description = description
        };
        
        var result = await client.Groups.PostAsync(request, cancellationToken: cancellationToken);
        return result ?? throw new InvalidOperationException("Failed to create group");
    }

    public async Task<GroupViewModel> UpdateAsync(int groupId, string name, string? description, CancellationToken cancellationToken = default)
    {
        var client = _clientFactory.CreateClient();
        var request = new UpdateGroupRequest
        {
            Name = name,
            Description = description
        };
        
        var result = await client.Groups[groupId].PutAsync(request, cancellationToken: cancellationToken);
        return result ?? throw new InvalidOperationException("Failed to update group");
    }

    public async Task DeleteAsync(int groupId, CancellationToken cancellationToken = default)
    {
        var client = _clientFactory.CreateClient();
        await client.Groups[groupId].DeleteAsync(cancellationToken: cancellationToken);
    }

    #endregion

    #region Membres

    public async Task<GroupViewModel> JoinAsync(string inviteCode, CancellationToken cancellationToken = default)
    {
        var client = _clientFactory.CreateClient();
        var request = new JoinGroupRequest
        {
            InviteCode = inviteCode
        };
        
        var result = await client.Groups.Join.PostAsync(request, cancellationToken: cancellationToken);
        return result ?? throw new InvalidOperationException("Failed to join group");
    }

    public async Task LeaveAsync(int groupId, CancellationToken cancellationToken = default)
    {
        var client = _clientFactory.CreateClient();
        await client.Groups[groupId].Leave.PostAsync(cancellationToken: cancellationToken);
    }

    public async Task RemoveMemberAsync(int groupId, string memberId, CancellationToken cancellationToken = default)
    {
        var client = _clientFactory.CreateClient();
        await client.Groups[groupId].Members[memberId].DeleteAsync(cancellationToken: cancellationToken);
    }

    public async Task TransferOwnershipAsync(int groupId, string newOwnerId, CancellationToken cancellationToken = default)
    {
        var client = _clientFactory.CreateClient();
        var request = new TransferOwnershipRequest
        {
            NewOwnerId = newOwnerId
        };
        
        await client.Groups[groupId].TransferOwnership.PostAsync(request, cancellationToken: cancellationToken);
    }

    #endregion

    #region Codes d'invitation

    public async Task<GroupInviteCodeViewModel> CreateInviteCodeAsync(int groupId, CancellationToken cancellationToken = default)
    {
        var client = _clientFactory.CreateClient();
        var result = await client.Groups[groupId].InviteCodes.PostAsync(cancellationToken: cancellationToken);
        return result ?? throw new InvalidOperationException("Failed to create invite code");
    }

    public async Task RevokeInviteCodeAsync(int groupId, int codeId, CancellationToken cancellationToken = default)
    {
        var client = _clientFactory.CreateClient();
        await client.Groups[groupId].InviteCodes[codeId].DeleteAsync(cancellationToken: cancellationToken);
    }

    #endregion
}
