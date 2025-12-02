using GameOnTonight.RestClient.Models;

namespace GameOnTonight.App.Services;

public interface IStatisticsService
{
    Task<StatisticsViewModel?> GetStatisticsAsync(CancellationToken cancellationToken = default);
}
