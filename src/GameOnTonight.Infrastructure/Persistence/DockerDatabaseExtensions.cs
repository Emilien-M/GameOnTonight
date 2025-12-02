using System.Diagnostics;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GameOnTonight.Infrastructure.Persistence;

public static class DockerDatabaseExtensions
{
    public static WebApplication MigrateDatabase<T>(this WebApplication app) where T : DbContext
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;
        var logger = services.GetRequiredService<ILogger<T>>();
        var context = services.GetRequiredService<T>();

        try
        {
            logger.LogInformation("Migrating database associated with context {DbContextName}", typeof(T).Name);
            context.Database.Migrate();
            logger.LogInformation("Database migration for context {DbContextName} completed.", typeof(T).Name);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while migrating the database for context {DbContextName}", typeof(T).Name);
        }

        return app;
    }

    public static IHostBuilder UseDockerDatabase(this IHostBuilder hostBuilder)
    {
        return hostBuilder.ConfigureServices((context, services) =>
        {
            if (context.HostingEnvironment.IsDevelopment())
            {
                var logger = services.BuildServiceProvider().GetRequiredService<ILogger<ApplicationDbContext>>();
                
                try 
                {
                    logger.LogInformation("Starting Docker PostgreSQL container for development...");
                    StartDockerPostgresContainer();
                    logger.LogInformation("PostgreSQL container started successfully.");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error starting PostgreSQL container.");
                }
            }
        });
    }

    private static void StartDockerPostgresContainer()
    {
        var projectDirectory = GetDockerComposeDirectory();
        
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "docker",
                Arguments = "compose up -d postgres",
                WorkingDirectory = projectDirectory,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            }
        };

        process.Start();
        var output = process.StandardOutput.ReadToEnd();
        var error = process.StandardError.ReadToEnd();
        process.WaitForExit();

        if (process.ExitCode != 0)
        {
            throw new Exception($"Error starting Docker container: {error}");
        }
    }

    private static string GetDockerComposeDirectory()
    {
        // Nouveau chemin du fichier docker-compose.yml dans le dossier Infrastructure/Docker
        var infrastructureDockerPath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory, 
            "..", "..", "..", "..", 
            "GameOnTonight.Infrastructure", 
            "Docker");
            
        if (Directory.Exists(infrastructureDockerPath) && 
            File.Exists(Path.Combine(infrastructureDockerPath, "docker-compose.yml")))
        {
            return Path.GetFullPath(infrastructureDockerPath);
        }

        // Fallback : chercher à d'autres emplacements courants
        var directory = new DirectoryInfo(Directory.GetCurrentDirectory());
        
        // Vérifier s'il existe un dossier Docker dans le répertoire courant
        var dockerDir = Path.Combine(directory.FullName, "Docker");
        if (Directory.Exists(dockerDir) && File.Exists(Path.Combine(dockerDir, "docker-compose.yml")))
        {
            return dockerDir;
        }
        
        // Vérifier dans le répertoire parent pour le dossier Infrastructure/Docker
        if (directory.Parent != null)
        {
            var parentInfrastructureDockerPath = Path.Combine(directory.Parent.FullName, "GameOnTonight.Infrastructure", "Docker");
            if (Directory.Exists(parentInfrastructureDockerPath) && 
                File.Exists(Path.Combine(parentInfrastructureDockerPath, "docker-compose.yml")))
            {
                return parentInfrastructureDockerPath;
            }
        }
        
        // Vérifier dans le répertoire src/GameOnTonight.Infrastructure/Docker
        if (directory.Name == "src" && directory.Parent != null)
        {
            var srcInfrastructureDockerPath = Path.Combine(directory.FullName, "GameOnTonight.Infrastructure", "Docker");
            if (Directory.Exists(srcInfrastructureDockerPath) && 
                File.Exists(Path.Combine(srcInfrastructureDockerPath, "docker-compose.yml")))
            {
                return srcInfrastructureDockerPath;
            }
        }
        
        // Vérifier l'ancienne localisation à la racine du projet
        if (directory.Parent != null && File.Exists(Path.Combine(directory.Parent.FullName, "docker-compose.yml")))
        {
            return directory.Parent.FullName;
        }

        throw new FileNotFoundException(
            "Could not find docker-compose.yml. It should be in the GameOnTonight.Infrastructure/Docker folder."
        );
    }
}
