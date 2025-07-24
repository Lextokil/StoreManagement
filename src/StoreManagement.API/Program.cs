using StoreManagement.API;
using StoreManagement.Domain.Interfaces.Services;
using StoreManagement.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddInfrastructure(builder.Configuration)
    .AddApi()
    .AddAutoMapper(typeof(Program));

// Configure Azure App Configuration
var appConfigEndpoint = builder.Configuration["AppConfigEndpoint"];

if (!string.IsNullOrEmpty(appConfigEndpoint))
{
    builder.Configuration.AddAzureAppConfiguration(options => { options.Connect(appConfigEndpoint); });
}


var app = builder.Build();

// Run database migrations automatically
using (var scope = app.Services.CreateScope())
{
    var migrationService = scope.ServiceProvider.GetRequiredService<IMigrationService>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    
    try
    {
        logger.LogInformation("Starting database migrations...");
        await migrationService.RunMigrationsAsync();
        logger.LogInformation("Database migrations completed successfully.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Failed to run database migrations. Application will continue but may not work properly.");
        // Continue running the application even if migrations fail
        // This allows for debugging and manual intervention
    }
}

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

app.Run();
