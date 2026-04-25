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

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        var FrontendUrl = builder.Configuration["FrontendUrl"];

        if (string.IsNullOrWhiteSpace(FrontendUrl) && (builder.Environment.IsEnvironment("Test") || builder.Environment.IsDevelopment()))
        {
            FrontendUrl = "http://localhost:5000";
        }

        if (string.IsNullOrWhiteSpace(FrontendUrl))
        {
            throw new InvalidOperationException(
                "FrontendUrl não configurado. Configure a variável FrontendUrl no ambiente.");
        }

        policy
            .WithOrigins(FrontendUrl)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var ConnectionString = GetConnectionString(builder);

if (!builder.Environment.IsDevelopment() && !builder.Environment.IsEnvironment("Test"))
{
    var DataSourcePrefix = "Data Source=";

    if (ConnectionString.StartsWith(DataSourcePrefix, StringComparison.OrdinalIgnoreCase))
    {
        var DbPath = ConnectionString[DataSourcePrefix.Length..];
        var DbDirectory = Path.GetDirectoryName(DbPath);

        if (!string.IsNullOrWhiteSpace(DbDirectory))
        {
            Directory.CreateDirectory(DbDirectory);
        }
    }
}

builder.Services.AddInfrastructure(ConnectionString);

builder.Services.AddScoped<IOrderService, OrderService>();

var app = builder.Build();

if (!app.Environment.IsEnvironment("Test"))
{
    using var Scope = app.Services.CreateScope();
    var Db = Scope.ServiceProvider.GetRequiredService<AppDbContext>();
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

static string GetConnectionString(WebApplicationBuilder builder)
{
    if (builder.Environment.IsProduction())
    {
        var ProductionConnectionString =
            Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
            ?? builder.Configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrWhiteSpace(ProductionConnectionString))
        {
            throw new InvalidOperationException(
                "Connection string de produção não configurada. Configure ConnectionStrings__DefaultConnection no Azure.");
        }

        return ProductionConnectionString;
    }

    var DevelopmentConnectionString =
        builder.Configuration.GetConnectionString("DefaultConnection")
        ?? builder.Configuration["DefaultConnection"]
        ?? "Data Source=goodhamburguer.db";

    return DevelopmentConnectionString;
}

public partial class Program { }