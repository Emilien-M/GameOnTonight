using GameOnTonight.App.Components.Groups;
using GameOnTonight.RestClient.Models;

namespace GameOnTonight.App.Services;

/// <summary>
/// Implémentation du service de contexte de groupe.
/// </summary>
public class GroupContextService : IGroupContextService
{
    private readonly IGroupsService _groupsService;
    private IReadOnlyList<GroupViewModel>? _cachedGroups;
    private DateTime _cacheExpiry = DateTime.MinValue;
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(5);

    public GroupContextService(IGroupsService groupsService)
    {
        _groupsService = groupsService;
    }

    /// <inheritdoc />
    public int? SelectedGroupId { get; private set; }
    
    /// <inheritdoc />
    public string? SelectedGroupName { get; private set; }
    
    /// <inheritdoc />
    public GroupFilterMode FilterMode { get; private set; } = GroupFilterMode.All;
    
    /// <inheritdoc />
    public event Action? OnFilterChanged;

    /// <inheritdoc />
    public void SetFilter(GroupFilterMode mode, int? groupId = null, string? groupName = null)
    {
        // Validation
        if (mode == GroupFilterMode.GroupOnly && !groupId.HasValue)
        {
            throw new ArgumentException("GroupId is required when FilterMode is GroupOnly", nameof(groupId));
        }

        var hasChanged = FilterMode != mode || SelectedGroupId != groupId;
        
        FilterMode = mode;
        SelectedGroupId = mode == GroupFilterMode.GroupOnly ? groupId : null;
        SelectedGroupName = mode == GroupFilterMode.GroupOnly ? groupName : null;
        
        if (hasChanged)
        {
            OnFilterChanged?.Invoke();
        }
    }
    
    /// <inheritdoc />
    public void ResetFilter()
    {
        SetFilter(GroupFilterMode.All);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<GroupViewModel>> GetUserGroupsAsync(CancellationToken cancellationToken = default)
    {
        // Vérifier si le cache est valide
        if (_cachedGroups is not null && DateTime.UtcNow < _cacheExpiry)
        {
            return _cachedGroups;
        }
        
        // Récupérer les groupes depuis l'API
        _cachedGroups = await _groupsService.GetMyGroupsAsync(cancellationToken);
        _cacheExpiry = DateTime.UtcNow.Add(CacheDuration);
        
        return _cachedGroups;
    }

    /// <inheritdoc />
    public void InvalidateGroupsCache()
    {
        _cachedGroups = null;
        _cacheExpiry = DateTime.MinValue;
    }
}
