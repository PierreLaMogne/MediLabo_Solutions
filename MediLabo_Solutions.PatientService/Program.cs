using MediLabo_Solutions.PatientService.Data;
using MediLabo_Solutions.PatientService.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configuration de la base de données
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configuration d'Identity
builder.Services.AddIdentityCore<IdentityUser>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// Configuration des controllers
builder.Services.AddControllers();

// Enregistrement des services
builder.Services.AddScoped<IPatientService, PatientService>();

var app = builder.Build();

// DataSeed lorsque la DB est vide au lancement
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<AppDbContext>();
    context.Database.Migrate();
    DataSeed.Seed(context);
}

app.MapControllers();

app.Run();
