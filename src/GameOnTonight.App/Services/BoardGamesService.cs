using System.Collections.ObjectModel;
using GameOnTonight.RestClient.BoardGames.Filter;
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

    public async Task<IReadOnlyList<BoardGameViewModel>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var client = _clientFactory.CreateClient();
            var items = await client.BoardGames.GetAsync(cancellationToken: cancellationToken);
            var result = items ?? new List<BoardGameViewModel>();
            
            // Cache the result for offline use
            await _cacheService.SetCachedBoardGamesAsync(result);
            
            return result;
        }
        catch (HttpRequestException)
        {
            // Network error - try to return cached data
            return await _cacheService.GetCachedBoardGamesAsync();
        }
    }

    public async Task<IReadOnlyList<BoardGameViewModel>> FilterAsync(int playersCount, int maxDurationMinutes, IReadOnlyList<string>? gameTypes, CancellationToken cancellationToken = default)
    {
        try
        {
            var client = _clientFactory.CreateClient();
            return (await client.BoardGames.Filter.GetAsync(config =>
            {
                config.QueryParameters = new FilterRequestBuilder.FilterRequestBuilderGetQueryParameters
                {
                    PlayersCount = playersCount,
                    MaxDurationMinutes = maxDurationMinutes,
                    GameTypes = gameTypes?.ToArray()
                };
            }, cancellationToken))?.AsReadOnly() ?? ReadOnlyCollection<BoardGameViewModel>.Empty;
        }
        catch (HttpRequestException)
        {
            // Network error - filter cached data locally
            var cachedGames = await _cacheService.GetCachedBoardGamesAsync();
            return FilterGamesLocally(cachedGames, playersCount, maxDurationMinutes, gameTypes);
        }
    }
    
    public async Task<BoardGameViewModel?> SuggestAsync(int playersCount, int maxDurationMinutes, IReadOnlyList<string>? gameTypes, CancellationToken cancellationToken = default)
    {
        try
        {
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
