using GoodHamburger.Domain.Entities;

namespace GoodHamburger.Domain.Interfaces;

public interface IOrderRepository
{
    Task<Order> GetByIdAsync(int Id);
    Task<IReadOnlyList<Order>> GetAllAsync();
    Task AddAsync(Order AddOrder);
    Task UpdateAsync(Order UpdateOrder);
    Task DeleteAsync(Order DeleteOrder);
}