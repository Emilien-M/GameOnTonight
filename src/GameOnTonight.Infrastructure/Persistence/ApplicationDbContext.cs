using GameOnTonight.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using GameOnTonight.Infrastructure.Configurations;
using GameOnTonight.Infrastructure.Interceptors;

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
        
        builder.ApplyConfiguration(new GameConfiguration());
    }
    
    // Définir ici les DbSet pour les entités métier
    public DbSet<Game> Games { get; set; }
}
