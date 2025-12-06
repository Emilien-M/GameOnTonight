using GameOnTonight.Domain.Entities;
using GameOnTonight.Domain.Entities.Common;
using GameOnTonight.Domain.Services;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using GameOnTonight.Infrastructure.Interceptors;
using GameOnTonight.Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore.Storage;

namespace GameOnTonight.Infrastructure.Persistence;

/// <summary>
/// The main application database context that extends IdentityDbContext
/// to integrate ASP.NET Core Identity.
/// </summary>
public class ApplicationDbContext : IdentityDbContext<IdentityUser>, ITransactionalElement
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

        builder.ApplyConfiguration(new ProfileConfiguration());
        builder.ApplyConfiguration(new BoardGameConfiguration());
        builder.ApplyConfiguration(new GameSessionConfiguration());
        builder.ApplyConfiguration(new GameSessionPlayerConfiguration());
        builder.ApplyConfiguration(new GameTypeConfiguration());
        builder.ApplyConfiguration(new GroupConfiguration());
        builder.ApplyConfiguration(new GroupMemberConfiguration());
        builder.ApplyConfiguration(new GroupInviteCodeConfiguration());
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        var newUsers = ChangeTracker.Entries<IdentityUser>()
            .Where(e => e.State == EntityState.Added)
            .Select(e => e.Entity)
            .ToList();

        foreach (var user in newUsers)
        {
            var displayName = user.UserName?.Split('@')[0] ?? "Nouveau Joueur";
                
            var profile = new Profile(displayName);
            profile.SetUserId(user.Id);
                
            await Profiles.AddAsync(profile, cancellationToken);
        }

        return await base.SaveChangesAsync(cancellationToken);
    }

    public DbSet<Profile> Profiles { get; set; }
    public DbSet<BoardGame> BoardGames { get; set; }
    public DbSet<GameSession> GameSessions { get; set; }
    public DbSet<GameSessionPlayer> GameSessionPlayers { get; set; }
    public DbSet<GameType> GameTypes { get; set; }
    public DbSet<Group> Groups { get; set; }
    public DbSet<GroupMember> GroupMembers { get; set; }
    public DbSet<GroupInviteCode> GroupInviteCodes { get; set; }
    
    public bool IsInTransaction { get; private set; }
    private IDbContextTransaction? _transaction;
    
    public async Task<ITransactionalElement> BeginTransactionAsync()
    {
        if (_transaction != null && IsInTransaction)
            return this;
        
        _transaction = await Database.BeginTransactionAsync();
        IsInTransaction = true;
        return this;
    }

    public async Task CommitAsync()
    {
        if (!IsInTransaction)
            return;

        try
        {
            await _transaction!.CommitAsync();
        }
        finally
        {
            IsInTransaction = false;
        }
    }

    public async Task RollbackAsync()
    {
        if (!IsInTransaction)
            return;

        try
        {
            await _transaction!.RollbackAsync();
        }
        finally
        {
            IsInTransaction = false;
        }
    }
}
