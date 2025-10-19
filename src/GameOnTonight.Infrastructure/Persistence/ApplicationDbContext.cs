using GameOnTonight.Domain.Entities;
using GameOnTonight.Domain.Services;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using GameOnTonight.Infrastructure.Interceptors;
using GameOnTonight.Infrastructure.Configurations;

namespace GameOnTonight.Infrastructure.Persistence;

/// <summary>
/// The main application database context that extends IdentityDbContext
/// to integrate ASP.NET Core Identity.
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

        builder.ApplyConfiguration(new ProfilConfiguration());
        builder.ApplyConfiguration(new BoardGameConfiguration());
        builder.ApplyConfiguration(new GameSessionConfiguration());
    }
    
    public DbSet<Profil> Profils { get; set; }
    public DbSet<BoardGame> BoardGames { get; set; }
    public DbSet<GameSession> GameSessions { get; set; }
}
