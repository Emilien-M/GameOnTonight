using System.Collections.ObjectModel;
using GameOnTonight.RestClient.BoardGames.Filter;
using GameOnTonight.RestClient.BoardGames.Suggest;
using GameOnTonight.RestClient.Models;

namespace GameOnTonight.App.Services;

public class BoardGamesService : IBoardGamesService
{
    private readonly GameOnTonightClientFactory _clientFactory;

    public BoardGamesService(GameOnTonightClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }

    public async Task<IReadOnlyList<BoardGameViewModel>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var client = _clientFactory.CreateClient();
        var items = await client.BoardGames.GetAsync(cancellationToken: cancellationToken);
        return items ?? new List<BoardGameViewModel>();
    }

    public async Task<IReadOnlyList<BoardGameViewModel>> FilterAsync(int playersCount, int maxDurationMinutes, string? gameType, CancellationToken cancellationToken = default)
    {
        var client = _clientFactory.CreateClient();
        return (await client.BoardGames.Filter.GetAsync(config =>
        {
            config.QueryParameters = new FilterRequestBuilder.FilterRequestBuilderGetQueryParameters
            {
                PlayersCount = playersCount,
                MaxDurationMinutes = maxDurationMinutes,
                GameType = gameType
            };
        }, cancellationToken))?.AsReadOnly() ?? ReadOnlyCollection<BoardGameViewModel>.Empty;
    }
    
    public async Task<BoardGameViewModel?> SuggestAsync(int playersCount, int maxDurationMinutes, string? gameType, CancellationToken cancellationToken = default)
    {
        var client = _clientFactory.CreateClient();
        return await client.BoardGames.Suggest.GetAsync(config =>
        {
            config.QueryParameters = new SuggestRequestBuilder.SuggestRequestBuilderGetQueryParameters
            {
                PlayersCount = playersCount,
                MaxDurationMinutes = maxDurationMinutes,
                GameType = gameType
            };
        }, cancellationToken);
    }
    
    public async Task<BoardGameViewModel?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = _clientFactory.CreateClient();
        return await client.BoardGames[id].GetAsync(cancellationToken: cancellationToken);
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
}
