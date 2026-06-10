using MediLabo_Solutions.NoteService.Configuration;
using MongoDB.Driver;

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

var app = builder.Build();

app.Run();
