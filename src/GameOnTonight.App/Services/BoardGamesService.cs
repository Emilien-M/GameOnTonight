using System.Collections.ObjectModel;
using GameOnTonight.RestClient.BoardGames;
using GameOnTonight.RestClient.BoardGames.Filter;
using GameOnTonight.RestClient.BoardGames.Item.Share;
using GameOnTonight.RestClient.BoardGames.Suggest;
using GameOnTonight.RestClient.Models;

namespace GameOnTonight.App.Services;

public class BoardGamesService : IBoardGamesService
{
    private readonly GameOnTonightClientFactory _clientFactory;
    private readonly IOfflineCacheService _cacheService;

    public BoardGamesService(GameOnTonightClientFactory clientFactory, IOfflineCacheService cacheService)
    {
        _clientFactory = clientFactory;
        _cacheService = cacheService;
    }

    public async Task<IReadOnlyList<BoardGameViewModel>> GetAllAsync(int? groupId = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var client = _clientFactory.CreateClient();
            var items = await client.BoardGames.GetAsync(config =>
            {
                config.QueryParameters = new BoardGamesRequestBuilder.BoardGamesRequestBuilderGetQueryParameters
                {
                    GroupId = groupId
                };
            }, cancellationToken: cancellationToken);
            var result = items ?? new List<BoardGameViewModel>();
            
            // Cache the result for offline use (only when no filter)
            if (!groupId.HasValue)
            {
                await _cacheService.SetCachedBoardGamesAsync(result);
            }
            
            return result;
        }
        catch (HttpRequestException)
        {
            // Network error - try to return cached data (only when no filter)
            if (!groupId.HasValue)
            {
                return await _cacheService.GetCachedBoardGamesAsync();
            }
            throw;
        }
    }

    public async Task<IReadOnlyList<BoardGameViewModel>> FilterAsync(int playersCount, int maxDurationMinutes, IReadOnlyList<string>? gameTypes, int? groupId = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var client = _clientFactory.CreateClient();
            var result = (await client.BoardGames.Filter.GetAsync(config =>
            {
                config.QueryParameters = new FilterRequestBuilder.FilterRequestBuilderGetQueryParameters
                {
                    PlayersCount = playersCount,
                    MaxDurationMinutes = maxDurationMinutes,
                    GameTypes = gameTypes?.ToArray()
                };
            }, cancellationToken))?.AsReadOnly() ?? ReadOnlyCollection<BoardGameViewModel>.Empty;
            
            // Filter by group if specified
            if (groupId.HasValue)
            {
                result = result.Where(g => g.GroupId == groupId.Value).ToList().AsReadOnly();
            }
            
            return result;
        }
        catch (HttpRequestException)
        {
            // Network error - filter cached data locally
            var cachedGames = await _cacheService.GetCachedBoardGamesAsync();
            return FilterGamesLocally(cachedGames, playersCount, maxDurationMinutes, gameTypes);
        }
    }
    
    public async Task<BoardGameViewModel?> SuggestAsync(int playersCount, int maxDurationMinutes, IReadOnlyList<string>? gameTypes, int? groupId = null, CancellationToken cancellationToken = default)
    {
        try
        {
            // If groupId is specified, get filtered results and pick randomly
            if (groupId.HasValue)
            {
                var filtered = await FilterAsync(playersCount, maxDurationMinutes, gameTypes, groupId, cancellationToken);
                if (filtered.Count == 0) return null;
                var randomIndex = Random.Shared.Next(filtered.Count);
                return filtered[randomIndex];
            }
            
            var client = _clientFactory.CreateClient();
            return await client.BoardGames.Suggest.GetAsync(config =>
            {
                config.QueryParameters = new SuggestRequestBuilder.SuggestRequestBuilderGetQueryParameters
                {
                    PlayersCount = playersCount,
                    MaxDurationMinutes = maxDurationMinutes,
                    GameTypes = gameTypes?.ToArray()
                };
            }, cancellationToken);
        }
        catch (HttpRequestException)
        {
            // Network error - suggest from cached data locally
            var cachedGames = await _cacheService.GetCachedBoardGamesAsync();
            var filtered = FilterGamesLocally(cachedGames, playersCount, maxDurationMinutes, gameTypes);
            
            if (filtered.Count == 0) return null;
            
            var randomIndex = Random.Shared.Next(filtered.Count);
            return filtered[randomIndex];
        }
    }
    
    public async Task<IReadOnlyList<string>> GetGameTypesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var client = _clientFactory.CreateClient();
            var result = await client.BoardGames.Types.GetAsync(cancellationToken: cancellationToken);
            var gameTypes = result?.AsReadOnly() ?? ReadOnlyCollection<string>.Empty;
            
            // Cache for offline use
            await _cacheService.SetCachedGameTypesAsync(gameTypes);
            
            return gameTypes;
        }
        catch (HttpRequestException)
        {
            // Network error - return cached types
            return await _cacheService.GetCachedGameTypesAsync();
        }
    }
    
    public async Task<BoardGameViewModel?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var client = _clientFactory.CreateClient();
            return await client.BoardGames[id].GetAsync(cancellationToken: cancellationToken);
        }
        catch (HttpRequestException)
        {
            // Try to find in cache
            var cachedGames = await _cacheService.GetCachedBoardGamesAsync();
            return cachedGames.FirstOrDefault(g => g.Id == id);
        }
    }

    public async Task<BoardGameViewModel?> CreateAsync(CreateBoardGameCommand request, CancellationToken cancellationToken = default)
    {
        var client = _clientFactory.CreateClient();
        return await client.BoardGames.PostAsync(request, cancellationToken: cancellationToken);
    }

    public async Task<BoardGameViewModel?> UpdateAsync(int id, UpdateBoardGameCommand request, CancellationToken cancellationToken = default)
    {
        var client = _clientFactory.CreateClient();
        return await client.BoardGames[id].PutAsync(request, cancellationToken: cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = _clientFactory.CreateClient();
        await client.BoardGames[id].DeleteAsync(cancellationToken: cancellationToken);
    }

    public async Task<BoardGameViewModel?> ShareWithGroupAsync(int boardGameId, int groupId, CancellationToken cancellationToken = default)
    {
        var client = _clientFactory.CreateClient();
        return await client.BoardGames[boardGameId].Share.PostAsync(config =>
        {
            config.QueryParameters = new ShareRequestBuilder.ShareRequestBuilderPostQueryParameters
            {
                GroupId = groupId
            };
        }, cancellationToken: cancellationToken);
    }

    public async Task<BoardGameViewModel?> UnshareAsync(int boardGameId, CancellationToken cancellationToken = default)
    {
        var client = _clientFactory.CreateClient();
        return await client.BoardGames[boardGameId].Unshare.PostAsync(cancellationToken: cancellationToken);
    }
    
    private static IReadOnlyList<BoardGameViewModel> FilterGamesLocally(
        IReadOnlyList<BoardGameViewModel> games,
        int playersCount,
        int maxDurationMinutes,
        IReadOnlyList<string>? gameTypes)
    {
        var filtered = games.Where(g =>
            g.MinPlayers <= playersCount &&
            g.MaxPlayers >= playersCount &&
            g.DurationMinutes <= maxDurationMinutes);

        if (gameTypes is { Count: > 0 })
        {
            filtered = filtered.Where(g => 
                g.GameTypes?.Any(gt => gameTypes.Contains(gt, StringComparer.OrdinalIgnoreCase)) == true);
        }

        return filtered.ToList().AsReadOnly();
    }
}
