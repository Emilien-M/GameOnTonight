using FluentValidation;
using GameOnTonight.Api.Config;
using GameOnTonight.Api.Middlewares;
using GameOnTonight.Application.Behaviors;
using GameOnTonight.Application.BoardGames.Queries;
using GameOnTonight.Infrastructure;
using GameOnTonight.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    var configuration = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development"}.json", optional: true)
        .AddEnvironmentVariables()
        .Build();

    var useInMemoryDatabase = configuration.GetSection("UseInMemoryDatabase").Get<bool>();

    if (useInMemoryDatabase)
    {
        options.UseInMemoryDatabase("GameOnTonight");   
    }
    else
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        options.UseNpgsql(connectionString);
    }
});
builder.Services.AddRepositories();
builder.Services.AddSingleton(TimeProvider.System);

builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
    options.OpenApiVersion = OpenApiSpecVersion.OpenApi3_0;
});

builder.Services.AddMediator(options =>
{
    options.Assemblies = [typeof(ValidationBehavior<,>), typeof(Program)];
    options.ServiceLifetime = ServiceLifetime.Scoped;
});

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policyBuilder =>
    {
        var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();
        
        if (allowedOrigins is { Length: > 0 })
        {
            policyBuilder
                .WithOrigins(allowedOrigins)
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
        }
        else
        {
            // Fallback for development if no origins configured
            policyBuilder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
        }
    });
});

builder.Services.AddAuthentication();
builder.Services.AddAuthorization();
builder.Services.AddIdentityApiEndpoints<IdentityUser>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddValidatorsFromAssemblyContaining<SuggestBoardGameQueryValidator>();

var app = builder.Build();

// Set base path for all routes (API will be accessible at /api/*)
app.UsePathBase("/api");

// Auto-migrate database if enabled
var autoMigrate = builder.Configuration.GetValue<bool>("AUTO_MIGRATE") ||
                  Environment.GetEnvironmentVariable("AUTO_MIGRATE")?.ToLowerInvariant() == "true";

if (autoMigrate)
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    
    // Skip migrations for InMemory database
    if (!dbContext.Database.IsInMemory())
    {
        logger.LogInformation("AUTO_MIGRATE is enabled. Applying pending migrations...");
        try
        {
            var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync();
            var pendingList = pendingMigrations.ToList();
            
            if (pendingList.Count > 0)
            {
                logger.LogInformation("Found {Count} pending migration(s): {Migrations}", 
                    pendingList.Count, string.Join(", ", pendingList));
                await dbContext.Database.MigrateAsync();
                logger.LogInformation("Database migrations applied successfully.");
            }
            else
            {
                logger.LogInformation("No pending migrations found. Database is up to date.");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while applying database migrations.");
            throw;
        }
    }
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<UnitOfWorkMiddleware>();

app.MapIdentityApi<IdentityUser>()
    .WithTags("Identity");
app.MapControllers();

// Health check endpoints
app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = TimeProvider.System.GetUtcNow().UtcDateTime }))
    .WithTags("Health")
    .AllowAnonymous();

app.MapGet("/ready", async (ApplicationDbContext dbContext) =>
{
    try
    {
        await dbContext.Database.CanConnectAsync();
        return Results.Ok(new { status = "ready", timestamp = DateTime.UtcNow });
    }
    catch
    {
        return Results.StatusCode(503);
    }
})
    .WithTags("Health")
    .AllowAnonymous();

await app.RunAsync();