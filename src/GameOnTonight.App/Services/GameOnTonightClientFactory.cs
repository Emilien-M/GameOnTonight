using GameOnTonight.RestClient;
using Microsoft.Kiota.Http.HttpClientLibrary;

namespace GameOnTonight.App.Services;

public class GameOnTonightClientFactory
{
    private readonly IAuthService _authService;
    private readonly IHttpClientFactory _httpClientFactory;

    public GameOnTonightClientFactory(IAuthService authService, IHttpClientFactory httpClientFactory)
    {
        _authService = authService;
        _httpClientFactory = httpClientFactory;
    }

    public GameOnTonightClient CreateClient()
    {
        // Use the HttpClient with refresh token handler
        var httpClient = _httpClientFactory.CreateClient("GameOnTonightApiWithRefresh");
        var authProvider = new BearerTokenAuthenticationProvider(_authService.GetTokenAsync);
        var adapter = new HttpClientRequestAdapter(authProvider, httpClient: httpClient);
        return new GameOnTonightClient(adapter);
    }
}