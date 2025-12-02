using System.Collections.ObjectModel;
using GameOnTonight.RestClient.GameSessions;
using GameOnTonight.RestClient.Models;

namespace GameOnTonight.App.Services;

public class GameSessionsService : IGameSessionsService
{
    private readonly GameOnTonightClientFactory _clientFactory;

    public GameSessionsService(GameOnTonightClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }

    public async Task<IReadOnlyList<GameSessionViewModel>> GetHistoryAsync(int? count = null, CancellationToken cancellationToken = default)
    {
        var client = _clientFactory.CreateClient();
        var result = await client.GameSessions.GetAsync(config =>
        {
            config.QueryParameters = new GameSessionsRequestBuilder.GameSessionsRequestBuilderGetQueryParameters
            {
                Count = count
            };
        }, cancellationToken);
        
        return result?.AsReadOnly() ?? ReadOnlyCollection<GameSessionViewModel>.Empty;
    }

    public async Task<GameSessionViewModel?> CreateAsync(CreateGameSessionCommand command, CancellationToken cancellationToken = default)
    {
        var client = _clientFactory.CreateClient();
        return await client.GameSessions.PostAsync(command, cancellationToken: cancellationToken);
    }

    public async Task<GameSessionViewModel?> UpdateAsync(int id, UpdateGameSessionCommand command, CancellationToken cancellationToken = default)
    {
        var client = _clientFactory.CreateClient();
        return await client.GameSessions[id].PutAsync(command, cancellationToken: cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = _clientFactory.CreateClient();
        await client.GameSessions[id].DeleteAsync(cancellationToken: cancellationToken);
    }
}
