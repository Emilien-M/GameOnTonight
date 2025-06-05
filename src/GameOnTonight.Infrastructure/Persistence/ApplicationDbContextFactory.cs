using GameOnTonight.Domain.Services;
using GameOnTonight.Infrastructure.Interceptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace GameOnTonight.Infrastructure.Persistence;

public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        
        // Configure the database provider for design-time
        optionsBuilder.UseNpgsql("Host=localhost;Database=gameontonight_dev;Username=postgres;Password=postgres");
        
        // Create mock services for design-time
        var mockAuditInterceptor = new AuditableEntityInterceptor(TimeProvider.System);
        var mockCurrentUserService = new DesignTimeCurrentUserService();
        
        return new ApplicationDbContext(
            optionsBuilder.Options,
            mockAuditInterceptor,
            mockCurrentUserService);
    }
    
    /// <summary>
    /// Mock implementation of ICurrentUserService for design-time operations.
    /// </summary>
    private class DesignTimeCurrentUserService : ICurrentUserService
    {
        public string? UserId => "design-time-user";
        public bool IsAuthenticated => false;
    }
}