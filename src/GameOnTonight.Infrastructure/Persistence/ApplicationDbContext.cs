using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace GameOnTonight.Infrastructure.Persistence;

/// <summary>
/// Contexte de base de données principal de l'application qui étend IdentityDbContext
/// pour intégrer ASP.NET Core Identity
/// </summary>
public class ApplicationDbContext : IdentityDbContext<IdentityUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        // Ajouter ici les configurations personnalisées pour les entités métier
        // Par exemple : builder.ApplyConfiguration(new GameConfiguration());
    }
    
    // Définir ici les DbSet pour les entités métier
    // Par exemple : public DbSet<Game> Games { get; set; }
}
