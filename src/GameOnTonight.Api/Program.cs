using GameOnTonight.Api.Config;
using GameOnTonight.Api.Middlewares;
using GameOnTonight.Application.Behaviors;
using GameOnTonight.Infrastructure;
using GameOnTonight.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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
        policyBuilder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

builder.Services.AddAuthentication();
builder.Services.AddAuthorization();
builder.Services.AddIdentityApiEndpoints<IdentityUser>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "v1");
    });
}

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<UnitOfWorkMiddleware>();

app.MapIdentityApi<IdentityUser>()
    .WithTags("Identity");
app.MapControllers();

await app.RunAsync();