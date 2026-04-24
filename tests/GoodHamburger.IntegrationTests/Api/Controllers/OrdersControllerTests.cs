using System.Net;
using System.Net.Http.Json;
using GoodHamburger.Application.DTOs;
using GoodHamburger.Domain.Exceptions;
using GoodHamburger.IntegrationTests.Fixtures;
using Microsoft.AspNetCore.Mvc;

namespace GoodHamburger.IntegrationTests.Api.Controllers;

public class OrdersControllerTests(IntegrationTestFactory Factory) : IClassFixture<IntegrationTestFactory>
{
    private readonly HttpClient _Client = Factory.CreateClient();

    #region POST /api/orders
    [Fact]
    public async Task CreateOrder_WithSandwichFriesDrink_Returns201_With20PercentDiscount()
    {
        // Arrange
        var Request = new { items = new[] { 1, 4, 5 } }; // XBurger + Fries + SoftDrink
        
        // Act
        var Response = await _Client.PostAsJsonAsync("/api/orders", Request);
        var Order = await Response.Content.ReadFromJsonAsync<OrderResponse>();
        
        // Assert
        Assert.NotNull(Order);
        Assert.Equal(3, Order.Items.Count);
        Assert.Equal(20m, Order.DiscountPercent);
        Assert.Equal(7.60m, Order.TotalAmount);
        Assert.Equal(HttpStatusCode.Created, Response.StatusCode);
    }
    
    [Fact]
    public async Task CreateOrder_WithSandwichAndDrink_Returns201_With15PercentDiscount()
    {
        // Arrange
        var Request = new { items = new[] { 2, 5 } }; // XEgg + SoftDrink
        var Response = await _Client.PostAsJsonAsync("/api/orders", Request);
        
        // Act
        var Order = await Response.Content.ReadFromJsonAsync<OrderResponse>();
        
        // Assert
        Assert.NotNull(Order);
        Assert.Equal(15m, Order.DiscountPercent);
        Assert.Equal(5.95m, Order.TotalAmount);
        Assert.Equal(HttpStatusCode.Created, Response.StatusCode);
    }
    
    [Fact]
    public async Task CreateOrder_WithSandwichAndFries_Returns201_With10PercentDiscount()
    {
        // Arrange
        var Request = new { items = new[] { 3, 4 } }; // XBacon + Fries
        var Response = await _Client.PostAsJsonAsync("/api/orders", Request);
        
        // Act
        var Order = await Response.Content.ReadFromJsonAsync<OrderResponse>();
        
        // Assert
        Assert.NotNull(Order);
        Assert.Equal(10m, Order.DiscountPercent);
        Assert.Equal(8.10m, Order.TotalAmount);
        Assert.Equal(HttpStatusCode.Created, Response.StatusCode);
    }
    
    [Fact]
    public async Task CreateOrder_WithNoDiscount_Returns201_WithFullPrice()
    {
        // Arrange
        var Request = new { items = new[] { 1 } }; // XBurger only
        var Response = await _Client.PostAsJsonAsync("/api/orders", Request);
        
        // Act
        var Order = await Response.Content.ReadFromJsonAsync<OrderResponse>();
        
        // Assert
        Assert.NotNull(Order);
        Assert.Equal(0m, Order.DiscountPercent);
        Assert.Equal(5.00m, Order.TotalAmount);
        Assert.Equal(HttpStatusCode.Created, Response.StatusCode);
    }
    
    [Fact]
    public async Task CreateOrder_ReturnsLocationHeader()
    {
        // Arrange
        var Request = new { items = new[] { 1 } };
        
        // Act
        var Response = await _Client.PostAsJsonAsync("/api/orders", Request);

        // Assert
        Assert.Equal(HttpStatusCode.Created, Response.StatusCode);
        Assert.NotNull(Response.Headers.Location);
    }
    
    [Fact]
    public async Task CreateOrder_WithEmptyItems_Returns404()
    {
        // Arrange
        var Request = new { items = Array.Empty<int>() };
        
        // Act
        var Response = await _Client.PostAsJsonAsync("/api/orders", Request);
        
        // Assert
        Assert.Equal(HttpStatusCode.NotFound, Response.StatusCode);
    }
    
    [Fact]
    public async Task CreateOrder_WithDuplicateItems_Returns409()
    {
        // Arrange
        var Request = new { items = new[] { 4, 4 } };
        
        // Act
        var Response = await _Client.PostAsJsonAsync("/api/orders", Request);

        // Assert
        Assert.Equal(HttpStatusCode.Conflict, Response.StatusCode);
    }
    
    [Fact]
    public async Task CreateOrder_WithTwoSandwiches_Returns409()
    {
        // Arrange
        var Request = new { items = new[] { 1, 2 } }; // XBurger + XEgg
        
        // Act
        var Response = await _Client.PostAsJsonAsync("/api/orders", Request);
        
        // Assert
        Assert.Equal(HttpStatusCode.Conflict, Response.StatusCode);
        
    }
    
    [Fact]
    public async Task CreateOrder_Error_ReturnsProperProblemDetails()
    {
        // Arrange
        var Request = new { items = new[] { 1, 2 } };

        // Act
        var Response = await _Client.PostAsJsonAsync("/api/orders", Request);
        var ProblemDetails  = await Response.Content.ReadFromJsonAsync<ProblemDetails>();

        // Assert
        Assert.Equal(HttpStatusCode.Conflict, Response.StatusCode);
        Assert.Equal("Duplicate item", ProblemDetails!.Title);
        Assert.Equal("Sandwiches must contain more than one item", ProblemDetails.Detail);
        Assert.Equal(409, ProblemDetails.Status);
    }
    #endregion

    #region GET /api/order/{id}

    [Fact]
    public async Task GetById_ExistingOrder_Returns200()
    {
        // Arrange
        var Created = await CreateOrderAsync(new[] { 1, 4, 5 });

        // Act
        var Response = await _Client.GetAsync($"/api/orders/{Created.Id}");
        var Order = await Response.Content.ReadFromJsonAsync<OrderResponse>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, Response.StatusCode);
        Assert.Equal(Created.Id, Order!.Id);
    }
    
    [Fact]
    public async Task GetById_NonExistingOrder_Returns404()
    {
        // Act
        var Response = await _Client.GetAsync("/api/orders/99999");
        var ProblemDetails  = await Response.Content.ReadFromJsonAsync<ProblemDetails>();
        
        // Assert
        Assert.Equal(HttpStatusCode.NotFound, Response.StatusCode);
        Assert.Equal("Order not found", ProblemDetails.Title);
        Assert.Equal("Order not found with id 99999", ProblemDetails.Detail);
    }
    #endregion

    #region DELETE /api/orders/{id}

    [Fact]
    public async Task Delete_ExistingOrder_Returns204()
    {
        // Arrange
        var Created = await CreateOrderAsync(new[] { 1 });

        // Act 
        var Response = await _Client.DeleteAsync($"/api/orders/{Created.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, Response.StatusCode);
    }

    [Fact]
    public async Task Delete_ExistingOrder_ThenGetById_Returns409()
    {
        // Arrange
        var Created = await CreateOrderAsync(new[] { 1 });
        
        // Act 
        await _Client.DeleteAsync($"/api/orders/{Created.Id}");
        var Response = await _Client.GetAsync($"/api/orders/{Created.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, Response.StatusCode);
    }

    [Fact]
    public async Task Delete_NonExisting_Returns404()
    {
        // Act 
        var Response = await _Client.DeleteAsync("/api/orders/9999");
        var ProblemDetails  = await Response.Content.ReadFromJsonAsync<ProblemDetails>();

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, Response.StatusCode);
        Assert.Equal("Order not found", ProblemDetails.Title);
        Assert.Equal("Order not found with id 9999", ProblemDetails.Detail);
    }
    #endregion
    
    #region PUT /api/orders/{id}
    
    [Fact]
    public async Task Update_ExistingOrder_Returns200_WithUpdatedData()
    {
        // Arrange
        var Created = await CreateOrderAsync(new[] { 1 }); // XBurger only, no discount

        // Act 
        var Response = await _Client.PutAsJsonAsync($"/api/orders/{Created.Id}", new { items = new[] { 3, 4, 5 } });
        var Order = await Response.Content.ReadFromJsonAsync<OrderResponse>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, Response.StatusCode);
        Assert.NotNull(Order);
        Assert.Equal(3, Order.Items.Count);
        Assert.Equal(20m, Order.DiscountPercent);
        Assert.Equal(9.20m, Order.TotalAmount);
    }
    
    [Fact]
    public async Task Update_ChangesDiscount_WhenItemsChange()
    {
        // Arrange
        var Created = await CreateOrderAsync(new[] { 1, 5 });
        
        // Act 
        var Response = await _Client.PutAsJsonAsync($"/api/orders/{Created.Id}", new { items = new[] { 1 } });
        var Order = await Response.Content.ReadFromJsonAsync<OrderResponse>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, Response.StatusCode);
        Assert.NotNull(Order);
        Assert.Equal(0m, Order.DiscountPercent);
        Assert.Equal(5.00m, Order.TotalAmount);
    }
    
    [Fact]
    public async Task Update_NonExisting_Returns404()
    {
        // Act 
        var Response = await _Client.PutAsJsonAsync("/api/orders/99999", new { items = new[] { 1 } });
        var ProblemDetails = await Response.Content.ReadFromJsonAsync<ProblemDetails>();
        
        // Assert
        Assert.Equal(HttpStatusCode.NotFound, Response.StatusCode);
        Assert.Equal("Order not found", ProblemDetails.Title);
        Assert.Equal("Order not found with id 99999", ProblemDetails.Detail);
    }
    
    [Fact]
    public async Task Update_WithDuplicateItems_Returns409()
    {
        // Arrange
        var Created = await CreateOrderAsync(new[] { 1 });
        
        // Act 
        var Response = await _Client.PutAsJsonAsync($"/api/orders/{Created.Id}", new { items = new[] { 4, 4 } });
        var ProblemDetails = await Response.Content.ReadFromJsonAsync<ProblemDetails>();

        // Assert
        Assert.Equal(HttpStatusCode.Conflict, Response.StatusCode);
        Assert.Equal("Duplicate item", ProblemDetails.Title);
        Assert.Equal("Fries must contain more than one item", ProblemDetails.Detail);
    }
    
    [Fact]
    public async Task Update_WithTwoSandwiches_Returns409()
    {
        // Arrange
        var Created = await CreateOrderAsync(new[] { 1 });
        
        // Act
        var Response = await _Client.PutAsJsonAsync($"/api/orders/{Created.Id}", new { items = new[] { 1, 2 } });
        var ProblemDetails = await Response.Content.ReadFromJsonAsync<ProblemDetails>();

        // Assert
        Assert.Equal(HttpStatusCode.Conflict, Response.StatusCode);
        Assert.Equal("Duplicate item", ProblemDetails.Title);
        Assert.Equal("Sandwiches must contain more than one item", ProblemDetails.Detail);
    }
    
    #endregion

    #region Get /api/orders

    [Fact]
    public async Task GetAll_Returns200_WithList()
    {
        // Arrange
        var CreatedA = await CreateOrderAsync(new[] { 1 });
        var CreatedB = await CreateOrderAsync(new[] { 2, 5 });

        // Act
        var Response = await _Client.GetAsync("/api/orders");
        var List = await Response.Content.ReadFromJsonAsync<List<OrderResponse>>();

        // Assert
        Assert.NotNull(List);
        Assert.Contains(List, o => o.Id == CreatedA.Id);
        Assert.Contains(List, o => o.Id == CreatedB.Id);
        Assert.Equal(HttpStatusCode.OK, Response.StatusCode);
    }
    
    [Fact]
    public async Task GetAll_ReturnsCorrectOrderData()
    {
        // Arrange
        var Created = await CreateOrderAsync(new[] { 1, 4, 5 });
        
        // Act
        var Response = await _Client.GetAsync("/api/orders");
        var List = await Response.Content.ReadFromJsonAsync<List<OrderResponse>>();
        var Order = List!.FirstOrDefault(O => O.Id == Created.Id);

        // Assert
        Assert.NotNull(Order);
        Assert.Equal(20m, Order.DiscountPercent);
        Assert.Equal(7.60m, Order.TotalAmount);
    }
    #endregion
    #region Helpers
    private async Task<OrderResponse> CreateOrderAsync(int[] items)
    {
        var Response = await _Client.PostAsJsonAsync("/api/orders", new { items });
        Response.EnsureSuccessStatusCode();
        return (await Response.Content.ReadFromJsonAsync<OrderResponse>())!;
    }

    #endregion
}