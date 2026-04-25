using GoodHamburger.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GoodHamburger.IntegrationTests.Fixtures;

public class IntegrationTestFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly SqliteConnection _connection = new("DataSource=:memory:");
    
    protected override void ConfigureWebHost(IWebHostBuilder Builder)
    {
        Builder.UseEnvironment("Test");
        
        Builder.ConfigureAppConfiguration((context, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["FrontendUrl"] = "http://localhost:5000"
            });
        });
        
        Builder.ConfigureServices(Services =>
        {
            var Descriptor = Services.SingleOrDefault(
                D => D.ServiceType == typeof(DbContextOptions<AppDbContext>));

            if (Descriptor != null)
                Services.Remove(Descriptor);

            Services.AddDbContext<AppDbContext>(opts =>
                opts.UseSqlite(_connection));
        });
    }

    public async Task InitializeAsync()
    {
        await _connection.OpenAsync();

        using var Scope = Services.CreateScope();
        var Db = Scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await Db.Database.EnsureCreatedAsync();
    }

    public new async Task DisposeAsync()
    {
        Environment.SetEnvironmentVariable("FrontendUrl", null);
        
        await _connection.CloseAsync();
        await base.DisposeAsync();
    }
}