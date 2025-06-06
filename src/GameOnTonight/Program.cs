using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using GameOnTonight;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Configuration du client HTTP pour communiquer avec l'API
// En développement, utilise l'URL de base du serveur hôte
// En production, les requêtes /api sont redirigées par Nginx vers le service API
builder.Services.AddScoped(sp => 
{
    var httpClient = new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) };
    
    // Ajouter des headers d'authentification si nécessaire
    // httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "...token...");
    
    return httpClient;
});

await builder.Build().RunAsync();