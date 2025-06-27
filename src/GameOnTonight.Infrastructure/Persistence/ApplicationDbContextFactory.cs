using System;
using GameOnTonight.Domain.Services;
using GameOnTonight.Infrastructure.Interceptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace GameOnTonight.Infrastructure.Persistence;

/// <summary>
/// Factory to create an instance of ApplicationDbContext at design time (migrations).
/// </summary>
public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    private readonly AuditableEntityInterceptor _interceptor;
    private readonly ICurrentUserService _currentUserService;

    public ApplicationDbContextFactory(AuditableEntityInterceptor interceptor, ICurrentUserService currentUserService)
    {
        _interceptor = interceptor;
        _currentUserService = currentUserService;
    }
    
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development"}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        builder.UseNpgsql(connectionString);

        return new ApplicationDbContext(builder.Options, _interceptor, _currentUserService);
    }
}
