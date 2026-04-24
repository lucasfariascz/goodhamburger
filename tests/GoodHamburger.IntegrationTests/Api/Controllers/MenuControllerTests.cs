using System.Net;
using System.Net.Http.Json;
using GoodHamburger.Application.DTOs;
using GoodHamburger.IntegrationTests.Fixtures;
using Microsoft.AspNetCore.Mvc;

namespace GoodHamburger.IntegrationTests.Api.Controllers;

public class MenuControllerTests(IntegrationTestFactory Factory) : IClassFixture<IntegrationTestFactory>
{
    private readonly HttpClient _Client = Factory.CreateClient();

    [Fact]
    public async Task GetMenu_Returns200_WithAllFiveItems()
    {
        // Act
        var Response = await _Client.GetAsync("/api/MenuItem");
        var Items = await Response.Content.ReadFromJsonAsync<List<MenuItemResponse>>();
        
        // Assert
        Assert.NotNull(Items);
        Assert.Equal(5, Items.Count);
        Assert.Equal(HttpStatusCode.OK, Response.StatusCode);
    }
    
    [Fact]
    public async Task GetMenu_Contains_XBurger_With_CorrectPrice()
    {
        // Act
        var Response = await _Client.GetAsync("/api/MenuItem");
        var Items = await Response.Content.ReadFromJsonAsync<List<MenuItemResponse>>();
        var xBurger = Items!.FirstOrDefault(I => I.Name == "XBurger");
        
        // Assert
        Assert.NotNull(xBurger);
        Assert.Equal(5.00m, xBurger.Price);
    }
    
    [Theory]
    [InlineData(1, "XBurger",   5.00)]
    [InlineData(2, "XEgg",      4.50)]
    [InlineData(3, "XBacon",    7.00)]
    [InlineData(4, "Fries",     2.00)]
    [InlineData(5, "SoftDrink", 2.50)]
    public async Task GetMenu_EachItem_HasCorrectIdNameAndPrice(int ExpectedId, string ExpectedName, decimal ExpectedPrice)
    {
        // Act
        var Response = await _Client.GetAsync("/api/MenuItem");
        var Items = await Response.Content.ReadFromJsonAsync<List<MenuItemResponse>>();
        var Item = Items!.FirstOrDefault(i => i.Id == ExpectedId);
        
        // Assert
        Assert.NotNull(Item);
        Assert.Equal(ExpectedName, Item.Name);
        Assert.Equal(ExpectedPrice, Item.Price);
    }
}