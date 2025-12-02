using GameOnTonight.RestClient.Models;

namespace GameOnTonight.App.Services;

public interface IBoardGamesService
{
    Task<IReadOnlyList<BoardGameViewModel>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<BoardGameViewModel>> FilterAsync(int playersCount, int maxDurationMinutes, string? gameType, CancellationToken cancellationToken = default);
    Task<BoardGameViewModel?> SuggestAsync(int playersCount, int maxDurationMinutes, string? gameType, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<string>> GetGameTypesAsync(CancellationToken cancellationToken = default);
    Task<BoardGameViewModel?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<BoardGameViewModel?> CreateAsync(CreateBoardGameCommand request, CancellationToken cancellationToken = default);
    Task<BoardGameViewModel?> UpdateAsync(int id, UpdateBoardGameCommand request, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}