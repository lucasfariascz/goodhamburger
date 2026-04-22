using GoodHamburger.Application.DTOs;

namespace GoodHamburger.Application.Interfaces;

public interface IOrderService
{
    Task<IReadOnlyList<OrderResponse>> GetAllAsync();
    
    Task<OrderResponse> GetByIdAsync(int Id);
    
    Task<OrderResponse> CreateAsync(CreateOrderRequest Request);
    
    Task<OrderResponse> UpdateAsync(int Id, UpdateOrderRequest Request);
    
    Task DeleteAsync(int Id);
}
