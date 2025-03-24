using AI_Symtop_checker.Repositories.Implementations;
using AI_Symtop_checker.Repositories.Interfaces;
using AI_Symtop_checker.Services.Implementations;
using AI_Symtop_checker.Services.Interfaces;
using AI_Symtop_checker.Configurations;
using MongoDB.Driver;
using Microsoft.Extensions.Options;




var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure MongoDB using IOptions pattern
builder.Services.Configure<MongoDbConfiguration>(options =>
{
    options.ConnectionString = builder.Configuration.GetConnectionString("MongoDB") ??
        throw new InvalidOperationException("MongoDB connection string is not configured");
    options.DatabaseName = "SymptomCheckerDb";
    options.SymptomsCollectionName = "Symptoms";
    options.SymptomCheckPredictionsCollectionName = "SymptomCheckPredictions";
    options.SymptomCheckRequestsCollectionName = "SymptomCheckRequests";
    options.LlamaApiResponsesCollectionName = "LlamaApiResponses";
});

// Register MongoDB client
builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var config = sp.GetRequiredService<IOptions<MongoDbConfiguration>>().Value;
    return new MongoClient(config.ConnectionString);
});

// Register HTTP client
builder.Services.AddHttpClient();

// Register Repositories
builder.Services.AddScoped<ISymptomRepository, MongoSymptomRepository>();
builder.Services.AddScoped<ILlamaIntegrationRepository, LlamaIntegrationRepository>();
builder.Services.AddScoped<IPredictionRepository, MongoPredictionRepository>(); // Add this line

// Register Services
builder.Services.AddScoped<ISymptomCheckerService, SymptomCheckerService>();
builder.Services.AddScoped<IAdminService, AdminService>(); // Add this line

// Register ModelService
builder.Services.AddSingleton<AI_Symtop_checker.ModelService.IModelService, AI_Symtop_checker.ModelService.PythonModelService>();

// Update LlamaIntegrationRepository registration
builder.Services.AddScoped<ILlamaIntegrationRepository>(sp => {
    var modelService = sp.GetRequiredService<AI_Symtop_checker.ModelService.IModelService>();
    var logger = sp.GetRequiredService<ILogger<LlamaIntegrationRepository>>();
    return new LlamaIntegrationRepository(modelService, logger);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();