using GoodHamburger.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GoodHamburger.IntegrationTests.Fixtures;

public class IntegrationTestFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly SqliteConnection _connection = new("DataSource=:memory:");
    
    protected override void ConfigureWebHost(IWebHostBuilder Builder)
    {
        Builder.ConfigureServices(Services =>
        {
            var Descriptor = Services.SingleOrDefault(
                D => D.ServiceType == typeof(DbContextOptions<AppDbContext>));

            if (Descriptor != null)
                Services.Remove(Descriptor);

            Services.AddDbContext<AppDbContext>(opts =>
                opts.UseSqlite(_connection));
        });

        Builder.UseEnvironment("Test");
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
        await _connection.CloseAsync();
        await base.DisposeAsync();
    }
}