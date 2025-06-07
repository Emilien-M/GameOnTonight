using System;
using GameOnTonight.Infrastructure.Interceptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace GameOnTonight.Infrastructure.Persistence;

/// <summary>
/// Factory pour créer une instance de ApplicationDbContext lors de la conception (migrations)
/// </summary>
public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        // Configuration pour charger la chaîne de connexion
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development"}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        builder.UseNpgsql(connectionString);
        
        // Créer une instance temporaire de l'intercepteur pour le contexte de design-time
        var auditInterceptor = new AuditableEntityInterceptor(TimeProvider.System);

        return new ApplicationDbContext(builder.Options, auditInterceptor);
    }
}
