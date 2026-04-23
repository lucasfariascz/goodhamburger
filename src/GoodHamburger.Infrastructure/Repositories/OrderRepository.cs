using GoodHamburger.Domain.Entities;
using GoodHamburger.Domain.Interfaces;
using GoodHamburger.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GoodHamburger.Infrastructure.Repositories;

public class OrderRepository(AppDbContext DbContext) : IOrderRepository
{
    public async Task<Order?> GetByIdAsync(int Id)
    {
        return await DbContext.Orders.Include(O => O.Items).FirstOrDefaultAsync(O => O.Id == Id);
    }

    public async Task<IReadOnlyList<Order>> GetAllAsync()
    {
        return await DbContext.Orders.Include(O => O.Items).ToListAsync();
    }

    public async Task AddAsync(Order AddOrder)
    {
        DbContext.Orders.Add(AddOrder);
        await DbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(Order UpdateOrder)
    {
        DbContext.Orders.Update(UpdateOrder);
        await DbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(Order DeleteOrder)
    {
        DbContext.Orders.Remove(DeleteOrder);
        await DbContext.SaveChangesAsync();
    }
}