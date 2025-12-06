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

// Resolve API base URL - supports relative paths (e.g., "/api") or absolute URLs
var configuredApiUrl = builder.Configuration["ApiBaseUrl"];
string apiBaseUrl;

if (string.IsNullOrWhiteSpace(configuredApiUrl))
{
    // Default to /api relative path when not configured
    apiBaseUrl = $"{builder.HostEnvironment.BaseAddress.TrimEnd('/')}/api";
}
else if (configuredApiUrl.StartsWith("/"))
{
    // Relative path - combine with base address
    apiBaseUrl = $"{builder.HostEnvironment.BaseAddress.TrimEnd('/')}{configuredApiUrl}";
}
else if (Uri.TryCreate(configuredApiUrl, UriKind.Absolute, out var parsedUri) &&
         (parsedUri.Scheme == Uri.UriSchemeHttp || parsedUri.Scheme == Uri.UriSchemeHttps))
{
    // Absolute URL - use as-is
    apiBaseUrl = configuredApiUrl;
}
else
{
    throw new InvalidOperationException(
        $"Configuration key 'ApiBaseUrl' must be either a relative path (e.g., '/api') or an absolute HTTP/HTTPS URL. Current value: '{configuredApiUrl}'.");
}

builder.Services.AddHttpClient("GameOnTonightApi", client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
});

builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped<IAuthService, AuthService>();

// Register the refresh token handler
builder.Services.AddScoped<RefreshTokenDelegatingHandler>();

// Configure the HttpClient with the refresh token handler
builder.Services.AddHttpClient("GameOnTonightApiWithRefresh", client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
}).AddHttpMessageHandler<RefreshTokenDelegatingHandler>();

builder.Services.AddScoped<GameOnTonightClientFactory>();
builder.Services.AddScoped<IOfflineCacheService, OfflineCacheService>();
builder.Services.AddScoped<IBoardGamesService, BoardGamesService>();
builder.Services.AddScoped<IGameSessionsService, GameSessionsService>();
builder.Services.AddScoped<IStatisticsService, StatisticsService>();
builder.Services.AddScoped<IProfileService, ProfileService>();
builder.Services.AddScoped<IErrorService, ErrorService>();
builder.Services.AddScoped<ISearchResultService, SearchResultService>();
builder.Services.AddScoped<IGroupsService, GroupsService>();
builder.Services.AddScoped<IGroupContextService, GroupContextService>();
builder.Services.AddLucideIcons();
builder.Services.AddMudServices();

await builder.Build().RunAsync();