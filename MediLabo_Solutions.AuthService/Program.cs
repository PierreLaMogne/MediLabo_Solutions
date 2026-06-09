using MediLabo_Solutions.ExceptionHandler.Extensions;
var builder = WebApplication.CreateBuilder(args);

// Configuration des controllers
builder.Services.AddControllers();

// Configuration des erreurs globales
builder.Services.AddProblemDetails();

var app = builder.Build();

// Middleware de gestion des exceptions globales
app.UseGlobalExceptionHandler();

app.UseRouting();
app.MapControllers();

app.Run();