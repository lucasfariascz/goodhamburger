using GoodHamburger.Application.DTOs;
using GoodHamburger.Application.Services;
using GoodHamburger.Domain;
using GoodHamburger.Domain.Entities;
using GoodHamburger.Domain.Exceptions;
using GoodHamburger.Domain.Interfaces;
using Moq;

namespace GoodHamburger.UnitTests.Application.Services;

public class OrderServiceTests
{
    private readonly Mock<IOrderRepository> _repoMock = new();
    private readonly OrderService _service;
    
    public OrderServiceTests()
    {
        _service = new OrderService(_repoMock.Object);
    }

    #region GetAllAsync
    [Fact]
    public async Task GetAllAsync_WhenNoOrders_ReturnsEmptyList()
    {
        // Arrange
        _repoMock.Setup(r => r.GetAllAsync())
            .ReturnsAsync([]);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        Assert.Empty(result);
        _repoMock.Verify(R => R.GetAllAsync(), Times.Once);
    }
    
    [Fact]
    public async Task GetAllAsync_ShouldReturnAllOrders_WithExpectedData()
    {
        // Arrange
        var Orders = new List<Order>
        {
            Order.Create(new[]
            {
                MenuItemCategory.XBurger,
                MenuItemCategory.Fries,
                MenuItemCategory.SoftDrink
            })
        };

        _repoMock
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(Orders);
        
        // Act
        var Result = await _service.GetAllAsync();
        
        // Assert
        Assert.Single(Result);
        Assert.Equal(3, Result[0].Items.Count);
        Assert.Equal(9.50m, Result[0].Subtotal);
        Assert.Equal(20m, Result[0].DiscountPercent);
        Assert.Equal(7.60m, Result[0].TotalAmount);
        _repoMock.Verify(R => R.GetAllAsync(), Times.Once);
    }
    #endregion

    #region GetByIdAsync
    [Fact]
    public async Task GetByIdAsync_ShouldReturnOrderWith_ExpectedOneData()
    {
        // Arrange
        var order = Order.Create(new[]
        {
            MenuItemCategory.XBurger
        });
        
        _repoMock
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(order);
    
        var Service = new OrderService(_repoMock.Object);
        
        // Act
        var Result = await Service.GetByIdAsync(1);
        
        // Assert
        Assert.Single(Result.Items);
        Assert.Equal("XBurger", Result.Items[0].Name);
        Assert.Equal(5.00m, Result.Items[0].Price);
        Assert.Equal(5.00m, Result.Subtotal);
        Assert.Equal(0m, Result.DiscountPercent);
        Assert.Equal(5.00m, Result.TotalAmount);
        _repoMock.Verify(R => R.GetByIdAsync(1), Times.Once);
    }
    
    [Fact]
    public async Task GetByIdAsync_ShouldThrowNotFoundException_WhenOrderDoesNotExist()
    {
        // Arrange and Act
        var Exception = await Assert.ThrowsAsync<DomainException>(() => _service.GetByIdAsync(999));
    
        // Assert
        Assert.IsType<DomainException>(Exception);
        Assert.Equal($"Order not found with id {999}", Exception.Message);
    }
    #endregion
    
    #region CreateAsync
    [Fact]
    public async Task CreateAsync_ShouldCreateOrder_WhenRequestIsValid()
    {
        // Arrange
        var Request = new CreateOrderRequest(new List<MenuItemCategory>
        {
            MenuItemCategory.XBurger,
            MenuItemCategory.Fries,
            MenuItemCategory.SoftDrink
        });
    
        // Act
        var Result = await _service.CreateAsync(Request);
    
        // Assert
        Assert.NotNull(Result);
        Assert.Equal(20m, Result.DiscountPercent);
        Assert.Equal(7.60m, Result.TotalAmount);
        Assert.Equal(3, Result.Items.Count);
        _repoMock.Verify(R => R.AddAsync(It.IsAny<Order>()), Times.Once);
    }
    
    [Fact]
    public async Task CreateAsync_WithSingleItem_WhenRequestIsValidNoDiscount()
    {
        // Arrange
        var Request = new CreateOrderRequest(new List<MenuItemCategory>
        {
            MenuItemCategory.XEgg
        });
        
        // Act
        var Result = await _service.CreateAsync(Request);

        // Assert
        Assert.Single(Result.Items);
        Assert.Equal("XEgg", Result.Items[0].Name);
        Assert.Equal(4.50m, Result.Items[0].Price);
        Assert.Equal(4.50m, Result.Subtotal);
        Assert.Equal(0m, Result.DiscountPercent);
        Assert.Equal(4.50m, Result.TotalAmount);
    }
    
    [Fact]
    public async Task CreateAsync_WithEmptyItems_WhenRequestThrowsDomainException()
    {
        // Arrange
        var Request = new CreateOrderRequest([]);
        
        // Act
        await Assert.ThrowsAsync<DomainException>(() => _service.CreateAsync(Request));
        
        // Assert
        _repoMock.Verify(R => R.AddAsync(It.IsAny<Order>()), Times.Never);
    }
    #endregion

    #region UpdateAsync
    [Fact]
    public async Task UpdateAsync_ShouldUpdateOrder_WhenRequestIsValid()
    {
        // Arrange
        var order = Order.Create([MenuItemCategory.XBurger]);
        _repoMock.Setup(R => R.GetByIdAsync(1))
            .ReturnsAsync(order);
        
        var Request = new UpdateOrderRequest(new List<MenuItemCategory>
        {
            MenuItemCategory.XEgg, MenuItemCategory.SoftDrink
        });
    
        // Act
        var Result = await _service.UpdateAsync(1, Request);
    
        // Assert
        Assert.NotNull(Result);
        Assert.Equal(2, Result.Items.Count);
        Assert.Equal(7.00m, Result.Subtotal);
        Assert.Equal(15m, Result.DiscountPercent);
        Assert.Equal(5.95m, Result.TotalAmount);
        
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowNotFoundException_WhenOrderDoesNotExist()
    {
        // Arrange
        _repoMock.Setup(r => r.GetByIdAsync(99))
            .ReturnsAsync((Order?)null);
        var Request = new UpdateOrderRequest(new List<MenuItemCategory>
        {
            MenuItemCategory.XEgg
        });
    
        // Act
        var Exception = await Assert.ThrowsAsync<DomainException>(() => _service.UpdateAsync(999, Request));
    
        // Assert
        Assert.IsType<DomainException>(Exception);
        Assert.Equal($"Order not found with id {999}", Exception.Message);
        _repoMock.Verify(R => R.UpdateAsync(It.IsAny<Order>()), Times.Never);
    }
    
    [Fact]
    public async Task UpdateAsync_ShouldThrowNotFoundException_WhenOrderNeverCalls()
    {
        // Arrange
        _repoMock.Setup(r => r.GetByIdAsync(99))
            .ReturnsAsync((Order?)null);
        
        var Request = new UpdateOrderRequest([]);
    
        // Act
        var Exception = await Assert.ThrowsAsync<DomainException>(() => _service.UpdateAsync(1, Request));
    
        // Assert
        Assert.IsType<DomainException>(Exception);
        Assert.Equal($"Order not found with id {1}", Exception.Message);
        _repoMock.Verify(R => R.UpdateAsync(It.IsAny<Order>()), Times.Never);
    }
    #endregion

    #region DeleteAsync
    [Fact]
    public async Task DeleteAsync_ShouldDeleteOrder_WhenRequestIsValid()
    {
        // Arrange
        var order = Order.Create([MenuItemCategory.XBurger]);
        _repoMock.Setup(R => R.GetByIdAsync(1))
            .ReturnsAsync(order);
        
        // Act
        await _service.DeleteAsync(1);
    
        // Assert
        _repoMock.Verify(R => R.DeleteAsync(order), Times.Once);
    }
    
    [Fact]
    public async Task DeleteAsync_ShouldThrowNotFoundException_WhenOrderDoesNotExist()
    {
        // Arrange
        _repoMock.Setup(r => r.GetByIdAsync(999))
            .ReturnsAsync((Order?)null);
    
        // Act
        var Exception = await Assert.ThrowsAsync<DomainException>(() => _service.DeleteAsync(999));
    
        // Assert
        Assert.IsType<DomainException>(Exception);
        Assert.Equal($"Order not found with id {999}", Exception.Message);
        _repoMock.Verify(R => R.DeleteAsync(It.IsAny<Order>()), Times.Never);
    }
    #endregion
    
}