using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using GameOnTonight;
using GameOnTonight.RestClient;
using GameOnTonight.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Blazored.LocalStorage;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddBlazoredLocalStorage();
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
builder.Services.AddScoped<IAuthService, AuthService>();

var apiBaseAddress = builder.HostEnvironment.IsDevelopment() 
    ? "http://localhost:5235" 
    : builder.HostEnvironment.BaseAddress;

builder.Services.AddScoped<AuthHeaderHandler>();

builder.Services.AddGameOnTonightApiClient(apiBaseAddress, null, clientBuilder =>
{
    clientBuilder.AddHttpMessageHandler<AuthHeaderHandler>();
});

await builder.Build().RunAsync();