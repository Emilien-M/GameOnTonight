using GameOnTonight.RestClient.Models;

namespace GameOnTonight.App.Services;

public interface ISearchResultService
{
    IReadOnlyList<BoardGameViewModel>? CurrentResult { get; }
    int PlayersCount { get; }
    int MaxDurationMinutes { get; }
    string? GameType { get; }
    
    void SetResult(IReadOnlyList<BoardGameViewModel> result, int playersCount, int maxDurationMinutes, string? gameType);
    void ClearResult();
}

public class SearchResultService : ISearchResultService
{
    public IReadOnlyList<BoardGameViewModel>? CurrentResult { get; private set; }
    public int PlayersCount { get; private set; }
    public int MaxDurationMinutes { get; private set; }
    public string? GameType { get; private set; }
    
    public void SetResult(IReadOnlyList<BoardGameViewModel> result, int playersCount, int maxDurationMinutes, string? gameType)
    {
        CurrentResult = result;
        PlayersCount = playersCount;
        MaxDurationMinutes = maxDurationMinutes;
        GameType = gameType;
    }
    
    public void ClearResult()
    {
        CurrentResult = null;
        PlayersCount = 0;
        MaxDurationMinutes = 0;
        GameType = null;
    }
}