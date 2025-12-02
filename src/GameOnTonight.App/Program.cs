using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using GameOnTonight.App;
using GameOnTonight.App.Services;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
});

var apiBaseUrl = builder.Configuration["ApiBaseUrl"];
if (string.IsNullOrWhiteSpace(apiBaseUrl))
{
    throw new InvalidOperationException("Configuration key 'ApiBaseUrl' is missing or empty. Define it in wwwroot/appsettings.json (or the environment-specific file) with an absolute HTTP/HTTPS URL to your backend API. Example: https://localhost:5001/");
}

if (!Uri.TryCreate(apiBaseUrl, UriKind.Absolute, out var parsedApiBaseUri) ||
    (parsedApiBaseUri.Scheme != Uri.UriSchemeHttp && parsedApiBaseUri.Scheme != Uri.UriSchemeHttps))
{
    throw new InvalidOperationException($"Configuration key 'ApiBaseUrl' must be an absolute HTTP/HTTPS URL. Current value: '{apiBaseUrl}'.");
}

builder.Services.AddHttpClient("GameOnTonightApi", client =>
{
    client.BaseAddress = new Uri(apiBaseUrl!);
});

builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped<IAuthService, AuthService>();

// Register the refresh token handler
builder.Services.AddScoped<RefreshTokenDelegatingHandler>();

// Configure the HttpClient with the refresh token handler
builder.Services.AddHttpClient("GameOnTonightApiWithRefresh", client =>
{
    client.BaseAddress = new Uri(apiBaseUrl!);
}).AddHttpMessageHandler<RefreshTokenDelegatingHandler>();

builder.Services.AddScoped<GameOnTonightClientFactory>();
builder.Services.AddScoped<IOfflineCacheService, OfflineCacheService>();
builder.Services.AddScoped<IBoardGamesService, BoardGamesService>();
builder.Services.AddScoped<IGameSessionsService, GameSessionsService>();
builder.Services.AddScoped<IStatisticsService, StatisticsService>();
builder.Services.AddScoped<IErrorService, ErrorService>();
builder.Services.AddScoped<ISearchResultService, SearchResultService>();
builder.Services.AddLucideIcons();
builder.Services.AddMudServices();

await builder.Build().RunAsync();