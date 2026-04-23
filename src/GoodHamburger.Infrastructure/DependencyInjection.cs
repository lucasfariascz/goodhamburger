using GoodHamburger.Domain.Interfaces;
using GoodHamburger.Infrastructure.Data;
using GoodHamburger.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GoodHamburger.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection Services, string ConnectionString)
    {
        Services.AddDbContext<AppDbContext>(Opts =>
            Opts.UseSqlite(ConnectionString));

        Services.AddScoped<IOrderRepository, OrderRepository>();

        return Services;
    }
}