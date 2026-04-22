using GoodHamburger.Application.DTOs;
using GoodHamburger.Domain;
using GoodHamburger.Domain.Entities;
using GoodHamburger.Domain.Exceptions;

namespace GoodHamburger.Application.Services;

public class OrderService
{
    public async Task<IReadOnlyList<OrderResponse>> GetAllAsync()
    {
        var orders = new List<OrderResponse>
        {
            new OrderResponse(
                1,
                new List<OrderItemResponse>
                {
                    new OrderItemResponse(1, MenuItemCategory.XBurger.ToString(), 5.00m),
                    new OrderItemResponse(2, MenuItemCategory.Fries.ToString(), 2.00m),
                    new OrderItemResponse(3, MenuItemCategory.SoftDrink.ToString(), 2.50m)
                },
                9.50m,   // Subtotal
                20m,     // DiscountPercent
                7.60m,   // TotalAmount (com desconto)
                DateTime.UtcNow,
                DateTime.UtcNow
            ),

            new OrderResponse(
                2,
                new List<OrderItemResponse>
                {
                    new OrderItemResponse(1, MenuItemCategory.XEgg.ToString(), 4.50m)
                },
                4.50m,
                0m,
                4.50m,
                DateTime.UtcNow,
                DateTime.UtcNow
            )
        };

        return await Task.FromResult(orders);
    }

    public Task<OrderResponse> GetByIdAsync(int Id)
    {
        var orders = new List<OrderResponse>
        {
            new OrderResponse(
                1,
                new List<OrderItemResponse>
                {
                    new OrderItemResponse(1, MenuItemCategory.XBurger.ToString(), 5.00m),
                    new OrderItemResponse(2, MenuItemCategory.Fries.ToString(), 2.00m),
                    new OrderItemResponse(3, MenuItemCategory.SoftDrink.ToString(), 2.50m)
                },
                9.50m,   // Subtotal
                20m,     // DiscountPercent
                7.60m,   // TotalAmount (com desconto)
                DateTime.UtcNow,
                DateTime.UtcNow
            ),

            new OrderResponse(
                2,
                new List<OrderItemResponse>
                {
                    new OrderItemResponse(1, MenuItemCategory.XEgg.ToString(), 4.50m)
                },
                4.50m,
                0m,
                4.50m,
                DateTime.UtcNow,
                DateTime.UtcNow
            )
        };

        var order = orders.FirstOrDefault(o => o.Id == Id);
        
        if (order == null) 
        {
            throw new DomainException($"Order not found with id {Id}");
        }
        
        return Task.FromResult(order);
    }

    public Task<OrderResponse> CreateAsync(CreateOrderRequest Request)
    {
        var OrderCreate = Order.Create(Request.Items);
        return Task.FromResult(MapToResponse(OrderCreate));
    }

    public Task<OrderResponse> UpdateAsync(int Id, UpdateOrderRequest Request)
    {
        var OrderFind = GetByIdAsync(Id).Result;
        if (OrderFind == null)
        {
            throw new DomainException($"Order not found with id {Id}");
        }

        var Order = new Order()
        {
            Id = OrderFind.Id,
            CreatedAt = OrderFind.CreatedAt,
            UpdatedAt = OrderFind.UpdatedAt,
            TotalAmount = OrderFind.TotalAmount,
            DiscountPercent = OrderFind.DiscountPercent,
        };
        
        Order.Update(Request.Items);
        
        return Task.FromResult(MapToResponse(Order));
    }
    
    public Task DeleteAsync(int Id)
    {
        var OrderFind = GetByIdAsync(Id).Result;
        if (OrderFind == null)
        {
            throw new DomainException($"Order not found with id {Id}");
        }

        return Task.CompletedTask;
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