using GameOnTonight.RestClient.Models;

namespace GameOnTonight.App.Services;

public interface ISearchResultService
{
    IReadOnlyList<BoardGameViewModel>? CurrentResult { get; }
    int PlayersCount { get; }
    int MaxDurationMinutes { get; }
    IReadOnlyList<string>? GameTypes { get; }
    
    void SetResult(IReadOnlyList<BoardGameViewModel> result, int playersCount, int maxDurationMinutes, IReadOnlyList<string>? gameTypes);
    void ClearResult();
}

public class SearchResultService : ISearchResultService
{
    public IReadOnlyList<BoardGameViewModel>? CurrentResult { get; private set; }
    public int PlayersCount { get; private set; }
    public int MaxDurationMinutes { get; private set; }
    public IReadOnlyList<string>? GameTypes { get; private set; }
    
    public void SetResult(IReadOnlyList<BoardGameViewModel> result, int playersCount, int maxDurationMinutes, IReadOnlyList<string>? gameTypes)
    {
        CurrentResult = result;
        PlayersCount = playersCount;
        MaxDurationMinutes = maxDurationMinutes;
        GameTypes = gameTypes;
    }
    
    public void ClearResult()
    {
        CurrentResult = null;
        PlayersCount = 0;
        MaxDurationMinutes = 0;
        GameTypes = null;
    }
}