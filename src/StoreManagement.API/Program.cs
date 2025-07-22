using Azure.Identity;
using Microsoft.EntityFrameworkCore;
using StoreManagement.Domain.Interfaces;
using StoreManagement.Infrastructure;
using StoreManagement.Infrastructure.Data;
using StoreManagement.Infrastructure.Repositories;
using StoreManagement.Infrastructure.Services;
using System.Reflection;
using StoreManagement.Domain.Interfaces.Repositories;
using StoreManagement.Domain.Interfaces.Services;

var builder = WebApplication.CreateBuilder(args);

// Configure Azure App Configuration and Key Vault
var keyVaultEndpoint = builder.Configuration["KeyVaultEndpoint"];
var appConfigEndpoint = builder.Configuration["AppConfigEndpoint"];

if (!string.IsNullOrEmpty(appConfigEndpoint))
{
    builder.Configuration.AddAzureAppConfiguration(options =>
    {
        options.Connect(appConfigEndpoint)
               .ConfigureKeyVault(kv =>
               {
                   if (!string.IsNullOrEmpty(keyVaultEndpoint))
                   {
                       kv.SetCredential(new DefaultAzureCredential());
                   }
               });
    });
}

if (!string.IsNullOrEmpty(keyVaultEndpoint))
{
    builder.Configuration.AddAzureKeyVault(
        new Uri(keyVaultEndpoint),
        new DefaultAzureCredential());
}

// Add services to the container
builder.Services.AddControllers();

// Configure Entity Framework
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? "Server=(localdb)\\mssqllocaldb;Database=StoreManagementDb;Trusted_Connection=true;MultipleActiveResultSets=true";

builder.Services.AddDbContext<StoreManagementDbContext>(options =>
    options.UseSqlServer(connectionString));

// Register repositories
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

// Add specific repositories
builder.Services.AddScoped<ICompanyRepository, CompanyRepository>();
builder.Services.AddScoped<IStoreRepository, StoreRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();

// Add Unit of Work
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Register services
builder.Services.AddScoped<IStoreService, StoreService>();
builder.Services.AddScoped<IProductService, ProductService>();

// Add AutoMapper
builder.Services.AddAutoMapper(typeof(Program));

// Configure Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { 
        Title = "Store Management API", 
        Version = "v1",
        Description = "A RESTful API for managing stores in a multi-company environment"
    });
    
    // Include XML comments
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Add health checks
builder.Services.AddHealthChecks()
    .AddSqlServer(connectionString);

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Store Management API V1");
        c.RoutePrefix = string.Empty; // Set Swagger UI at the app's root
    });
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("/health");

// Note: Database migrations are now handled by the StoreManagement.Database project
// Run the following command to apply migrations:
// dotnet run --project src/StoreManagement.Database

app.Run();
