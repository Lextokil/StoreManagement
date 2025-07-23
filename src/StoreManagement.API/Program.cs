using StoreManagement.API;
using StoreManagement.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddInfrastructure(builder)
    .AddApi()
    .AddAutoMapper(typeof(Program));

// Configure Azure App Configuration
var appConfigEndpoint = builder.Configuration["AppConfigEndpoint"];

if (!string.IsNullOrEmpty(appConfigEndpoint))
{
    builder.Configuration.AddAzureAppConfiguration(options => { options.Connect(appConfigEndpoint); });
}


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