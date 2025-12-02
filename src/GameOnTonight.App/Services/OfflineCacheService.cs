using Blazored.LocalStorage;
using GameOnTonight.RestClient.Models;

namespace GameOnTonight.App.Services;

/// <summary>
/// Interface for local caching of board game data for offline support.
/// </summary>
public interface IOfflineCacheService
{
    Task<IReadOnlyList<BoardGameViewModel>> GetCachedBoardGamesAsync();
    Task SetCachedBoardGamesAsync(IReadOnlyList<BoardGameViewModel> boardGames);
    Task<IReadOnlyList<string>> GetCachedGameTypesAsync();
    Task SetCachedGameTypesAsync(IReadOnlyList<string> gameTypes);
    Task ClearCacheAsync();
    Task<bool> HasCachedDataAsync();
    Task<DateTime?> GetLastSyncDateAsync();
}

/// <summary>
/// Service for caching board game data locally using LocalStorage for offline support.
/// </summary>
public class OfflineCacheService : IOfflineCacheService
{
    private readonly ILocalStorageService _localStorage;
    
    private const string BoardGamesCacheKey = "cache.boardGames";
    private const string GameTypesCacheKey = "cache.gameTypes";
    private const string LastSyncKey = "cache.lastSync";

    public OfflineCacheService(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }

    public async Task<IReadOnlyList<BoardGameViewModel>> GetCachedBoardGamesAsync()
    {
        try
        {
            var cached = await _localStorage.GetItemAsync<List<BoardGameViewModel>>(BoardGamesCacheKey);
            return cached?.AsReadOnly() ?? new List<BoardGameViewModel>().AsReadOnly();
        }
        catch
        {
            return new List<BoardGameViewModel>().AsReadOnly();
        }
    }

    public async Task SetCachedBoardGamesAsync(IReadOnlyList<BoardGameViewModel> boardGames)
    {
        await _localStorage.SetItemAsync(BoardGamesCacheKey, boardGames.ToList());
        await _localStorage.SetItemAsync(LastSyncKey, DateTime.UtcNow);
    }

    public async Task<IReadOnlyList<string>> GetCachedGameTypesAsync()
    {
        try
        {
            var cached = await _localStorage.GetItemAsync<List<string>>(GameTypesCacheKey);
            return cached?.AsReadOnly() ?? new List<string>().AsReadOnly();
        }
        catch
        {
            return new List<string>().AsReadOnly();
        }
    }

    public async Task SetCachedGameTypesAsync(IReadOnlyList<string> gameTypes)
    {
        await _localStorage.SetItemAsync(GameTypesCacheKey, gameTypes.ToList());
    }

    public async Task ClearCacheAsync()
    {
        await _localStorage.RemoveItemAsync(BoardGamesCacheKey);
        await _localStorage.RemoveItemAsync(GameTypesCacheKey);
        await _localStorage.RemoveItemAsync(LastSyncKey);
    }

    public async Task<bool> HasCachedDataAsync()
    {
        return await _localStorage.ContainKeyAsync(BoardGamesCacheKey);
    }

    public async Task<DateTime?> GetLastSyncDateAsync()
    {
        try
        {
            if (await _localStorage.ContainKeyAsync(LastSyncKey))
            {
                return await _localStorage.GetItemAsync<DateTime>(LastSyncKey);
            }
        }
        catch
        {
            // Ignore errors
        }
        return null;
    }
}
