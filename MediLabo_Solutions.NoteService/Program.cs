using MediLabo_Solutions.ExceptionHandler.Extensions;
using MediLabo_Solutions.NoteService.Configuration;
using MediLabo_Solutions.NoteService.Domain;
using MediLabo_Solutions.NoteService.Repositories;
using MediLabo_Solutions.NoteService.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Configuration MongoDB
builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDbSettings"));

builder.Services.AddSingleton<IMongoClient>(serviceProvider =>
{
    var settings = builder.Configuration.GetSection("MongoDbSettings").Get<MongoDbSettings>();
    return new MongoClient(settings!.ConnectionString);
});

builder.Services.AddScoped(serviceProvider =>
{
    var settings = builder.Configuration.GetSection("MongoDbSettings").Get<MongoDbSettings>();
    var client = serviceProvider.GetRequiredService<IMongoClient>();
    return client.GetDatabase(settings!.DatabaseName);
});

builder.Services.AddScoped<IMongoCollection<Note>>(serviceProvider =>
{
    var database = serviceProvider.GetRequiredService<IMongoDatabase>();
    var settings = builder.Configuration.GetSection("MongoDbSettings").Get<MongoDbSettings>();
    return database.GetCollection<Note>(settings!.NotesCollectionName);
});

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

// Enregistrement des services et repositories
builder.Services.AddScoped<INoteRepository, NoteRepository>();
builder.Services.AddScoped<INoteAppService, NoteAppService>();

var app = builder.Build();

// Middleware de gestion des exceptions
app.UseGlobalExceptionHandler();

// Configuration HTTPS redirection si nécessaire
// app.UseHttpsRedirection(); // Décommentez si vous voulez forcer HTTPS

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
