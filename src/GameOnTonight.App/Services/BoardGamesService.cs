using GameOnTonight.RestClient;
using GameOnTonight.RestClient.Models;

namespace GameOnTonight.App.Services;

public class BoardGamesService
{
    private readonly GameOnTonightClientFactory _clientFactory;

    public BoardGamesService(GameOnTonightClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }

    public async Task<IReadOnlyList<BoardGameViewModel>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var client = _clientFactory.CreateClient();
        var items = await client.BoardGames.GetAsync(cancellationToken: cancellationToken);
        return (IReadOnlyList<BoardGameViewModel>)(items ?? new List<BoardGameViewModel>());
    }

    public async Task<BoardGameViewModel?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = _clientFactory.CreateClient();
        return await client.BoardGames[id].GetAsync(cancellationToken: cancellationToken);
    }

    public async Task<BoardGameViewModel?> CreateAsync(CreateBoardGameCommand request, CancellationToken cancellationToken = default)
    {
        var client = _clientFactory.CreateClient();
        return await client.BoardGames.PostAsync(request, cancellationToken: cancellationToken);
    }

    public async Task<BoardGameViewModel?> UpdateAsync(int id, UpdateBoardGameCommand request, CancellationToken cancellationToken = default)
    {
        var client = _clientFactory.CreateClient();
        return await client.BoardGames[id].PutAsync(request, cancellationToken: cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = _clientFactory.CreateClient();
        await client.BoardGames[id].DeleteAsync(cancellationToken: cancellationToken);
    }
}
