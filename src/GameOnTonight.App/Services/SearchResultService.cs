using GameOnTonight.RestClient.Models;

namespace GameOnTonight.App.Services;

public interface ISearchResultService
{
    IReadOnlyList<BoardGameViewModel>? CurrentResult { get; set; }
    void SetResult(IReadOnlyList<BoardGameViewModel> result);
    void ClearResult();
}

public class SearchResultService : ISearchResultService
{
    public IReadOnlyList<BoardGameViewModel>? CurrentResult { get; set; }
    
    public void SetResult(IReadOnlyList<BoardGameViewModel> result)
    {
        CurrentResult = result;
    }
    
    public void ClearResult()
    {
        CurrentResult = null;
    }
}