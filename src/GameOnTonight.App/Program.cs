using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using GameOnTonight.App;
using GameOnTonight.App.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
});

builder.Services.AddHttpClient("GameOnTonightApi", client =>
{
    var apiBaseUrl = builder.Configuration["ApiBaseUrl"]!;
    client.BaseAddress = new Uri(apiBaseUrl);
});

builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<GameOnTonightClientFactory>();
builder.Services.AddScoped<IBoardGamesService, BoardGamesService>();
builder.Services.AddScoped<IErrorService, ErrorService>();
builder.Services.AddLucideIcons();

await builder.Build().RunAsync();