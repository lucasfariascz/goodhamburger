using GoodHamburger.Application.DTOs;
using GoodHamburger.Domain.Entities;
using GoodHamburger.Domain.Exceptions;
using GoodHamburger.Domain.Interfaces;

namespace GoodHamburger.Application.Services;

public class OrderService(IOrderRepository _orderRepository)
{
    public async Task<IReadOnlyList<OrderResponse>> GetAllAsync()
    {
        var Orders = await _orderRepository.GetAllAsync();
        return Orders.Select(MapToResponse).ToList();
    }

    public async Task<OrderResponse> GetByIdAsync(int Id)
    {
        var Order = await _orderRepository.GetByIdAsync(Id);
        
        if (Order == null) 
        {
            throw new DomainException($"Order not found with id {Id}");
        }
        
        return MapToResponse(Order);
    }

    public async Task<OrderResponse> CreateAsync(CreateOrderRequest Request)
    {
        var OrderCreate = Order.Create(Request.Items);
        await _orderRepository.AddAsync(OrderCreate);
        return MapToResponse(OrderCreate);
    }

    public async Task<OrderResponse> UpdateAsync(int Id, UpdateOrderRequest Request)
    {
        var Order = await _orderRepository.GetByIdAsync(Id);
        if (Order == null)
        {
            throw new DomainException($"Order not found with id {Id}");
        }
        
        Order.Update(Request.Items);
        
        return MapToResponse(Order);
    }
    
    public async Task DeleteAsync(int Id)
    {
        var Order = await _orderRepository.GetByIdAsync(Id);
        
        if (Order == null)
        {
            throw new DomainException($"Order not found with id {Id}");
        }
        
        await _orderRepository.DeleteAsync(Order);
    }
    
    private static OrderResponse MapToResponse(Order order)
    {
        var items = order.Items.Select(i => new OrderItemResponse(
            i.Id,
            i.Item.ToString(),
            Order.GetPrice(i.Item)
        )).ToList();

        var subtotal = items.Sum(I => I.Price);

        return new OrderResponse(
            order.Id,
            items,
            subtotal,
            order.DiscountPercent,
            order.TotalAmount,
            order.CreatedAt,
            order.UpdatedAt);
    }
}