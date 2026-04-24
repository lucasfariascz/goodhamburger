using GoodHamburger.Api.Middleware;
using GoodHamburger.Application.Interfaces;
using GoodHamburger.Application.Services;
using GoodHamburger.Infrastructure;
using GoodHamburger.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var ConnectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                       ?? "Data Source=goodhamburguer.db";

builder.Services.AddInfrastructure(ConnectionString);
builder.Services.AddScoped<IOrderService, OrderService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var Db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    Db.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseMiddleware<ExceptionMiddleware>();

app.MapControllers();

app.Run();

public partial class Program { }
