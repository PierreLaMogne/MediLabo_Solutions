using MediLabo_Solutions.AuthService.Data;
using MediLabo_Solutions.AuthService.Services;
using MediLabo_Solutions.ExceptionHandler.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);

// Configuration de la base de données
builder.Services.AddDbContext<AuthDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("AuthDb")));

// Configuration d'Identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;

    // Configuration des options de verrouillage
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    options.User.RequireUniqueEmail = false;
})
.AddEntityFrameworkStores<AuthDbContext>()
.AddDefaultTokenProviders();

// Enregistrement du service d'authentification
builder.Services.AddScoped<IAuthAppService, AuthAppService>();

// Configuration des controllers
builder.Services.AddControllers();

// Configuration des erreurs globales
builder.Services.AddProblemDetails();

var app = builder.Build();

// Initialisation de la base de données et création de l'utilisateur admin
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AuthDbContext>();
        await context.Database.MigrateAsync();
        await DbInitializer.InitializeUserAsync(services);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

// Middleware de gestion des exceptions globales
app.UseGlobalExceptionHandler();

app.UseRouting();
app.MapControllers();

app.Run();