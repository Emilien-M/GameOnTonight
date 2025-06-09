using System.Text;
using GameOnTonight.Api.Middlewares;
using GameOnTonight.Application.Auth.Commands;
using GameOnTonight.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using GameOnTonight.Infrastructure.Persistence;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

// Configurer Docker pour PostgreSQL en environnement de développement
builder.Host.UseDockerDatabase();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi(options =>
{
    options.OpenApiVersion = OpenApiSpecVersion.OpenApi3_0;
});

// Configurer les services DB et Identity
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Ajouter les repositories et l'UnitOfWork
builder.Services.AddRepositories();

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
    {
        // Configurer les options d'identité selon les besoins
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = true;
        options.Password.RequiredLength = 8;
        options.User.RequireUniqueEmail = true;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Configurer l'authentification JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        ValidAudience = builder.Configuration["JwtSettings:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Secret"]))
    };
});

// Configurer CORS pour accepter les requêtes du frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("DevelopmentPolicy", policy =>
    {
        policy.AllowAnyOrigin() // Permissive for local development
              .AllowAnyHeader()
              .AllowAnyMethod();
    });

    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("https://yourfrontenddomain.com",  // Replace with your actual production frontend URL
                           "http://localhost:3000",          // Common Blazor local dev URL via Docker frontend
                           "https://localhost:3001"          // Common Blazor local https dev URL (if applicable)
                           )
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // If your frontend sends credentials (e.g., cookies, auth headers)
                                   // and you need to support them with CORS.
                                   // If not, this can be omitted.
                                   // If using .AllowCredentials(), .AllowAnyOrigin() cannot be used for this policy.
    });
});

// Configuration pour le proxy et le forwarded headers
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedFor | 
                                Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedProto;
});

builder.Services.AddMediator(options =>
{
    options.Assemblies = [typeof(RegisterUserCommand), typeof(Program)];
    options.ServiceLifetime = ServiceLifetime.Scoped;
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// Appliquer automatiquement les migrations
app.MigrateDatabase<ApplicationDbContext>();

// Configuration pour gérer les forwarded headers du proxy
app.UseForwardedHeaders();

// Register global exception handling middleware early in the pipeline
app.UseGlobalExceptionHandler();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseCors("DevelopmentPolicy");
}
else
{
    app.UseHsts();
    app.UseCors("AllowFrontend");
}

// Ajout du support des fichiers statiques (pour Swagger UI)
app.UseStaticFiles();

// Configuration de l'OpenAPI pour tous les environnements
app.MapOpenApi("/openapi/v1.json");

// app.UseHttpsRedirection(); // Commenté car nous utilisons HTTP entre les conteneurs

// Ajouter le middleware UnitOfWork avant l'authentification et l'autorisation
app.UseUnitOfWork();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

await app.RunAsync();