using GameOnTonight.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using GameOnTonight.Infrastructure.Interceptors;
using GameOnTonight.Infrastructure.Configurations;

namespace GameOnTonight.Infrastructure.Persistence;

/// <summary>
/// Contexte de base de données principal de l'application qui étend IdentityDbContext
/// pour intégrer ASP.NET Core Identity
/// </summary>
public class ApplicationDbContext : IdentityDbContext<IdentityUser>
{
    private readonly AuditableEntityInterceptor _auditableEntityInterceptor;

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        AuditableEntityInterceptor auditableEntityInterceptor)
        : base(options)
    {
        _auditableEntityInterceptor = auditableEntityInterceptor;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(_auditableEntityInterceptor);
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        // Appliquer les configurations d'entités
        builder.ApplyConfiguration(new BoardGameConfiguration());
        builder.ApplyConfiguration(new GameSessionConfiguration());
    }
    
    // Définir ici les DbSet pour les entités métier
    public DbSet<BoardGame> BoardGames { get; set; }
    public DbSet<GameSession> GameSessions { get; set; }
}
