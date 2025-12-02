using GameOnTonight.RestClient.Models;

namespace GameOnTonight.App.Services;

public class StatisticsService : IStatisticsService
{
    private readonly GameOnTonightClientFactory _clientFactory;

    public StatisticsService(GameOnTonightClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }

    public async Task<StatisticsViewModel?> GetStatisticsAsync(CancellationToken cancellationToken = default)
    {
        var client = _clientFactory.CreateClient();
        return await client.Statistics.GetAsync(cancellationToken: cancellationToken);
    }
}
