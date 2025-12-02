using GameOnTonight.RestClient.Models;

namespace GameOnTonight.App.Services;

public interface IGameSessionsService
{
    Task<IReadOnlyList<GameSessionViewModel>> GetHistoryAsync(int? count = null, CancellationToken cancellationToken = default);
    Task<GameSessionViewModel?> CreateAsync(CreateGameSessionCommand command, CancellationToken cancellationToken = default);
    Task<GameSessionViewModel?> UpdateAsync(int id, UpdateGameSessionCommand command, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
