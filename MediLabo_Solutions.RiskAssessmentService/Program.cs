using MediLabo_Solutions.RiskAssessmentService.Handlers;
using MediLabo_Solutions.RiskAssessmentService.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

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

// Ajouter IHttpContextAccessor pour accéder au contexte HTTP actuel
builder.Services.AddHttpContextAccessor();

// Enregistrer le handler pour la propagation des tokens JWT
builder.Services.AddTransient<JwtTokenPropagationHandler>();

// Configuration des services et repositories
builder.Services.AddScoped<IRiskAssessmentAppService, RiskAssessmentAppService>();

// Configuration HTTP Client pour les services PatientService et NoteService avec propagation JWT
builder.Services.AddHttpClient("PatientService", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Services:PatientService:BaseUrl"]!);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
})
.AddHttpMessageHandler<JwtTokenPropagationHandler>();

builder.Services.AddHttpClient("NoteService", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Services:NoteService:BaseUrl"]!);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
})
.AddHttpMessageHandler<JwtTokenPropagationHandler>();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
