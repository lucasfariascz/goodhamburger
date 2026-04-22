using GoodHamburger.Application.DTOs;
using GoodHamburger.Application.Services;
using GoodHamburger.Domain;
using GoodHamburger.Domain.Exceptions;

namespace GoodHamburger.UnitTests.Application.Services;

public class OrderServiceTests
{
    [Fact]
    public async Task GetAllAsync_ShouldReturnAllOrders_WithExpectedData()
    {
        // Arrange
        var service = new OrderService();

        // Act
        var result = await service.GetAllAsync();
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        
        var FirstOrder = result[0];
        Assert.Equal(1, FirstOrder.Id);
        Assert.Equal(3, FirstOrder.Items.Count);
        Assert.Equal(9.50m, FirstOrder.Subtotal);
        Assert.Equal(20m, FirstOrder.DiscountPercent);
        Assert.Equal(7.60m, FirstOrder.TotalAmount);

        Assert.Equal(1, FirstOrder.Items[0].Id);
        Assert.Equal(MenuItemCategory.XBurger.ToString(), FirstOrder.Items[0].Name);
        Assert.Equal(5.00m, FirstOrder.Items[0].Price);

        Assert.Equal(2, FirstOrder.Items[1].Id);
        Assert.Equal(MenuItemCategory.Fries.ToString(), FirstOrder.Items[1].Name);
        Assert.Equal(2.00m, FirstOrder.Items[1].Price);

        Assert.Equal(3, FirstOrder.Items[2].Id);
        Assert.Equal(MenuItemCategory.SoftDrink.ToString(), FirstOrder.Items[2].Name);
        Assert.Equal(2.50m, FirstOrder.Items[2].Price);
        
        var SecondOrder = result[1];
        Assert.Equal(2, SecondOrder.Id);
        Assert.Single(SecondOrder.Items);
        Assert.Equal(4.50m, SecondOrder.Subtotal);
        Assert.Equal(0m, SecondOrder.DiscountPercent);
        Assert.Equal(4.50m, SecondOrder.TotalAmount);

        Assert.Equal(1, SecondOrder.Items[0].Id);
        Assert.Equal(MenuItemCategory.XEgg.ToString(), SecondOrder.Items[0].Name);
        Assert.Equal(4.50m, SecondOrder.Items[0].Price);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnOrderWith_ExpectedOneData()
    {
        // Arrange
        var service = new OrderService();

        // Act
        var result = await service.GetByIdAsync(1);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal(3, result.Items.Count);
        Assert.Equal(9.50m, result.Subtotal);
        Assert.Equal(20m, result.DiscountPercent);
        Assert.Equal(7.60m, result.TotalAmount);
    }
    
    [Fact]
    public async Task GetByIdAsync_ShouldThrowNotFoundException_WhenOrderDoesNotExist()
    {
        // Arrange
        var service = new OrderService();

        // Act
        var Exception = await Assert.ThrowsAsync<DomainException>(() => service.GetByIdAsync(999));

        // Assert
        Assert.IsType<DomainException>(Exception);
        Assert.Equal($"Order not found with id {999}", Exception.Message);
    }
    
    [Fact]
    public async Task CreateAsync_ShouldCreateOrder_WhenRequestIsValid()
    {
        // Arrange
        var service = new OrderService(); 
        var request = new CreateOrderRequest(new List<MenuItemCategory>
        {
            MenuItemCategory.XBurger,
            MenuItemCategory.Fries,
            MenuItemCategory.SoftDrink
        });

        // Act
        var result = await service.CreateAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(20m, result.DiscountPercent);
        Assert.Equal(7.60m, result.TotalAmount);
        Assert.Equal(3, result.Items.Count);
    }
    
    [Fact]
    public async Task UpdateAsync_ShouldUpdateOrder_WhenRequestIsValid()
    {
        // Arrange
        var service = new OrderService(); 
        var request = new UpdateOrderRequest(new List<MenuItemCategory>
        {
            MenuItemCategory.XEgg
        });

        // Act
        var result = await service.UpdateAsync(1, request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(0m, result.DiscountPercent);
        Assert.Equal(4.50m, result.TotalAmount);
        Assert.Single(result.Items);
    }
    
    [Fact]
    public async Task UpdateAsync_ShouldThrowNotFoundException_WhenOrderDoesNotExist()
    {
        // Arrange
        var service = new OrderService(); 
        var request = new UpdateOrderRequest(new List<MenuItemCategory>
        {
            MenuItemCategory.XEgg
        });

        // Act
        var Exception = await Assert.ThrowsAsync<DomainException>(() => service.UpdateAsync(999, request));

        // Assert
        Assert.IsType<DomainException>(Exception);
        Assert.Equal($"Order not found with id {999}", Exception.Message);
    }
}