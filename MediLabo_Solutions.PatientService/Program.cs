using MediLabo_Solutions.ExceptionHandler.Extensions;
using MediLabo_Solutions.PatientService.Data;
using MediLabo_Solutions.PatientService.Repositories;
using MediLabo_Solutions.PatientService.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IO.Compression;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Configuration de la base de données
builder.Services.AddDbContext<PatientDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configuration de l'authentification JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
    };
});

builder.Services.AddAuthorization();

// Ajouter HttpContextAccessor pour accéder au token JWT
builder.Services.AddHttpContextAccessor();

// Configuration du HttpClient pour NoteService
builder.Services.AddHttpClient<INoteServiceClient, NoteServiceClient>(client =>
{
    var noteServiceUrl = builder.Configuration["ServiceUrls:NoteService"] ?? "http://noteservice:8080";
    client.BaseAddress = new Uri(noteServiceUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
});

// Ajouter la compression de réponse
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<BrotliCompressionProvider>();
    options.Providers.Add<GzipCompressionProvider>();
});

builder.Services.Configure<BrotliCompressionProviderOptions>(options =>
{
    options.Level = CompressionLevel.Optimal;
});

builder.Services.Configure<GzipCompressionProviderOptions>(options =>
{
    options.Level = CompressionLevel.Optimal;
});

// Configuration des controllers avec gestion automatique des validations
builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        // Personnalisation optionnelle de la réponse de validation
        options.InvalidModelStateResponseFactory = context =>
        {
            var problemDetails = new ValidationProblemDetails(context.ModelState)
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                Title = "Erreur de validation",
                Status = StatusCodes.Status400BadRequest,
                Detail = "Une ou plusieurs erreurs de validation se sont produites.",
                Instance = context.HttpContext.Request.Path
            };

            return new BadRequestObjectResult(problemDetails)
            {
                ContentTypes = { "application/problem+json" }
            };
        };
    });

builder.Services.AddProblemDetails();

builder.Services.AddMemoryCache();

// Configuration des services et repositories
builder.Services.AddScoped<IPatientRepository, PatientRepository>();
builder.Services.AddScoped<IPatientAppService, PatientAppService>();

var app = builder.Build();

app.UseResponseCompression();

// Appliquer les migrations au démarrage (pour tous les environnements)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    
    var maxRetries = 10;
    var delay = TimeSpan.FromSeconds(5);
    
    for (int i = 0; i < maxRetries; i++)
    {
        try
        {
            var context = services.GetRequiredService<PatientDbContext>();
            
            logger.LogInformation("Tentative {Attempt}/{MaxRetries} - Initialisation de la base de données...", i + 1, maxRetries);
            
            // Appliquer toutes les migrations (créera la base si nécessaire)
            await context.Database.MigrateAsync();
            logger.LogInformation("Migrations appliquées avec succès.");
            
            // Vérifier que la connexion fonctionne
            var canConnect = await context.Database.CanConnectAsync();
            if (!canConnect)
            {
                throw new Exception("Impossible de se connecter à la base de données après migration.");
            }
            
            logger.LogInformation("Connexion à la base de données vérifiée.");
            
            // DataSeed uniquement en environnement Development
            if (app.Environment.IsDevelopment())
            {
                logger.LogInformation("Initialisation des données de test...");
                await DataSeed.SeedAsync(context);
                logger.LogInformation("Données de test initialisées avec succès.");
            }
            
            break; // Succès, sortir de la boucle
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Tentative {Attempt}/{MaxRetries} - Échec de l'initialisation.", i + 1, maxRetries);
            
            if (i == maxRetries - 1)
            {
                logger.LogError(ex, "Impossible d'initialiser la base de données après {MaxRetries} tentatives.", maxRetries);
                throw;
            }
            
            logger.LogInformation("Nouvelle tentative dans {Delay} secondes...", delay.TotalSeconds);
            await Task.Delay(delay);
        }
    }
}

// Middleware de gestion des exceptions
app.UseGlobalExceptionHandler();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
