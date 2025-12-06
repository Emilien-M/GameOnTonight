using GameOnTonight.RestClient.Models;

namespace GameOnTonight.App.Services;

public interface IBoardGamesService
{
    Task<IReadOnlyList<BoardGameViewModel>> GetAllAsync(int? groupId = null, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<BoardGameViewModel>> FilterAsync(int playersCount, int maxDurationMinutes, IReadOnlyList<string>? gameTypes, int? groupId = null, CancellationToken cancellationToken = default);
    Task<BoardGameViewModel?> SuggestAsync(int playersCount, int maxDurationMinutes, IReadOnlyList<string>? gameTypes, int? groupId = null, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<string>> GetGameTypesAsync(CancellationToken cancellationToken = default);
    Task<BoardGameViewModel?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<BoardGameViewModel?> CreateAsync(CreateBoardGameCommand request, CancellationToken cancellationToken = default);
    Task<BoardGameViewModel?> UpdateAsync(int id, UpdateBoardGameCommand request, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<BoardGameViewModel?> ShareWithGroupAsync(int boardGameId, int groupId, CancellationToken cancellationToken = default);
    Task<BoardGameViewModel?> UnshareAsync(int boardGameId, CancellationToken cancellationToken = default);
}