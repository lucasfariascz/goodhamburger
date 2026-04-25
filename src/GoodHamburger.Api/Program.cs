using GoodHamburger.Api.Middleware;
using GoodHamburger.Application.Interfaces;
using GoodHamburger.Application.Services;
using GoodHamburger.Infrastructure;
using GoodHamburger.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "GoodHamburger API", Version = "v1" });
});

var frontendUrl = builder.Configuration["FrontendUrl"];

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy
            .WithOrigins(frontendUrl!)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

if (!builder.Environment.IsDevelopment())
{
    var connectionString = builder.Configuration.GetConnectionString("ConnectionStrings__DefaultConnection");

    if (!string.IsNullOrWhiteSpace(connectionString))
    {
        var dataSourcePrefix = "Data Source=";

        if (connectionString.StartsWith(dataSourcePrefix, StringComparison.OrdinalIgnoreCase))
        {
            var dbPath = connectionString[dataSourcePrefix.Length..];
            var dbDirectory = Path.GetDirectoryName(dbPath);

            if (!string.IsNullOrWhiteSpace(dbDirectory))
            {
                Directory.CreateDirectory(dbDirectory);
            }
        }
    }
}
else
{
    var ConnectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                           ?? "Data Source=goodhamburguer.db";
    builder.Services.AddInfrastructure(ConnectionString);
}

builder.Services.AddScoped<IOrderService, OrderService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var Db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    Db.Database.Migrate();
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseMiddleware<ExceptionMiddleware>();

app.UseHttpsRedirection();

app.UseCors("Frontend");

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }
