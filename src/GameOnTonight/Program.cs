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

// Ajouter les services d'authentification
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
builder.Services.AddScoped<IAuthService, AuthService>();

// Configuration de l'URL de base de l'API
var apiBaseAddress = builder.HostEnvironment.IsDevelopment() 
    ? "http://localhost:5235" 
    : builder.HostEnvironment.BaseAddress;

// Enregistrement du gestionnaire d'autorisation
builder.Services.AddScoped<AuthorizationMessageHandler>();

// Enregistrement du client API avec la méthode d'extension du projet RestClient
// et configuration pour utiliser le gestionnaire d'autorisation
builder.Services.AddGameOnTonightApiClient(apiBaseAddress, configureClient: client => {
    // Configuration additionnelle du client HTTP
    client.DefaultRequestHeaders.Add("Accept", "application/json");
}, handler: sp => sp.GetRequiredService<AuthorizationMessageHandler>());

await builder.Build().RunAsync();