using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using GameOnTonight;
using GameOnTonight.RestClient;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Configuration de l'URL de base de l'API
var apiBaseAddress = builder.HostEnvironment.IsDevelopment() 
    ? "http://localhost:5235" 
    : builder.HostEnvironment.BaseAddress;

// Enregistrement du client API avec la méthode d'extension du projet RestClient
builder.Services.AddGameOnTonightApiClient(apiBaseAddress, client => {
    // Configuration additionnelle du client HTTP si nécessaire
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

await builder.Build().RunAsync();