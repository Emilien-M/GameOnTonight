using System.Collections.ObjectModel;
using GameOnTonight.RestClient.GameSessions;
using GameOnTonight.RestClient.GameSessions.Item.Players.Item.Link;
using GameOnTonight.RestClient.GameSessions.Item.Share;
using GameOnTonight.RestClient.Models;

namespace GameOnTonight.App.Services;

public class GameSessionsService : IGameSessionsService
{
    private readonly GameOnTonightClientFactory _clientFactory;

    public GameSessionsService(GameOnTonightClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }

    public async Task<IReadOnlyList<GameSessionViewModel>> GetHistoryAsync(int? count = null, int? groupId = null, CancellationToken cancellationToken = default)
    {
        var client = _clientFactory.CreateClient();
        var result = await client.GameSessions.GetAsync(config =>
        {
            config.QueryParameters = new GameSessionsRequestBuilder.GameSessionsRequestBuilderGetQueryParameters
            {
                Count = count,
                GroupId = groupId
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

    public async Task<GameSessionViewModel?> ShareWithGroupAsync(int sessionId, int groupId, CancellationToken cancellationToken = default)
    {
        var client = _clientFactory.CreateClient();
        return await client.GameSessions[sessionId].Share.PostAsync(config =>
        {
            config.QueryParameters = new ShareRequestBuilder.ShareRequestBuilderPostQueryParameters
            {
                GroupId = groupId
            };
        }, cancellationToken: cancellationToken);
    }

    public async Task<GameSessionViewModel?> UnshareAsync(int sessionId, CancellationToken cancellationToken = default)
    {
        var client = _clientFactory.CreateClient();
        return await client.GameSessions[sessionId].Unshare.PostAsync(cancellationToken: cancellationToken);
    }

    public async Task<GameSessionPlayerViewModel?> LinkPlayerToGroupMemberAsync(int sessionId, int playerId, int groupMemberId, CancellationToken cancellationToken = default)
    {
        var client = _clientFactory.CreateClient();
        return await client.GameSessions[sessionId].Players[playerId].Link.PostAsync(config =>
        {
            config.QueryParameters = new LinkRequestBuilder.LinkRequestBuilderPostQueryParameters
            {
                GroupMemberId = groupMemberId
            };
        }, cancellationToken: cancellationToken);
    }
}
