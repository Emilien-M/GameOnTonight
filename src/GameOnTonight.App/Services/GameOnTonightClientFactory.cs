using GameOnTonight.RestClient;
using Microsoft.Kiota.Http.HttpClientLibrary;

namespace GameOnTonight.App.Services;

public class GameOnTonightClientFactory
{
    private readonly AuthService _authService;
    private readonly IHttpClientFactory _httpClientFactory;

    public GameOnTonightClientFactory(AuthService authService, IHttpClientFactory httpClientFactory)
    {
        _authService = authService;
        _httpClientFactory = httpClientFactory;
    }

    public GameOnTonightClient CreateClient()
    {
        var httpClient = _httpClientFactory.CreateClient("GameOnTonightApi");
        var authProvider = new BearerTokenAuthenticationProvider(_authService.GetTokenAsync);
        var adapter = new HttpClientRequestAdapter(authProvider, httpClient: httpClient);
        return new GameOnTonightClient(adapter);
    }
}