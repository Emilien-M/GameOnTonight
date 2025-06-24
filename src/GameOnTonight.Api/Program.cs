using System.Text;
using GameOnTonight.Api.Config.Middlewares;
using GameOnTonight.Api.Config.OpenApi;
using GameOnTonight.Application.Auth.Commands;
using GameOnTonight.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using GameOnTonight.Infrastructure.Persistence;
using Microsoft.OpenApi;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Configurer Docker pour PostgreSQL en environnement de développement
builder.Host.UseDockerDatabase();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi(options =>
{
    options.OpenApiVersion = OpenApiSpecVersion.OpenApi3_0;
    options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
    options.AddDocumentTransformer<SecurityRequirementsTransformer>();
    options.AddDocumentTransformer<OperationIdTransformer>();
});

// Configurer les services DB et Identity
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Ajouter les repositories et l'UnitOfWork
builder.Services.AddRepositories();

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
    {
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
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
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

// Ajouter le middleware de gestion globale des erreurs en premier dans le pipeline
app.UseErrorHandling();

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

// Configuration de l'OpenAPI pour tous les environnements
app.MapOpenApi("/openapi/v1.json");
app.MapScalarApiReference();

// app.UseHttpsRedirection(); // Commenté car nous utilisons HTTP entre les conteneurs

app.UseUnitOfWork();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

await app.RunAsync();