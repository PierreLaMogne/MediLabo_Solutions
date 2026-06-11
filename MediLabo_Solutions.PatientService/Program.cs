using MediLabo_Solutions.PatientService.Data;
using MediLabo_Solutions.ExceptionHandler.Extensions;
using MediLabo_Solutions.PatientService.Repositories;
using MediLabo_Solutions.PatientService.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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

// Configuration des services et repositories
builder.Services.AddScoped<IPatientRepository, PatientRepository>();
builder.Services.AddScoped<IPatientAppService, PatientAppService>();

var app = builder.Build();

// DataSeed lorsque la DB est vide au lancement
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<PatientDbContext>();
    context.Database.Migrate();
    DataSeed.Seed(context);
}

// Middleware de gestion des exceptions
app.UseGlobalExceptionHandler();

// Configuration HTTPS redirection si nécessaire
// app.UseHttpsRedirection(); // Décommentez si vous voulez forcer HTTPS

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
