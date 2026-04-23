using GoodHamburger.Domain;
using GoodHamburger.Domain.Entities;

namespace GoodHamburger.UnitTests.Domain.Services;

public class DiscountCalculatorTests
{
    [Fact]
    public void Calculate_ShouldApply20Percent_WhenHasSandwichFriesAndDrink()
    {
        // Arrange and Act
        var OrderCreate = Order.Create([MenuItemCategory.XBurger, MenuItemCategory.Fries, MenuItemCategory.SoftDrink]);

        // Assert
        Assert.Equal(20m, OrderCreate.DiscountPercent);
        Assert.Equal(7.60m, OrderCreate.TotalAmount);
    }
    
    [Fact]
    public void Calculate_ShouldApply15Percent_WhenHasSandwichAndDrink()
    {
        // Arrange and Act
        var OrderCreate = Order.Create([MenuItemCategory.XBurger, MenuItemCategory.SoftDrink]);

        // Assert
        Assert.Equal(15m, OrderCreate.DiscountPercent);
        Assert.Equal(6.38m, OrderCreate.TotalAmount);
    }
    
    [Fact]
    public void Calculate_ShouldApply10Percent_WhenHasSandwichAndFries()
    {
        // Arrange and Act
        var OrderCreate = Order.Create([MenuItemCategory.XEgg, MenuItemCategory.Fries]);
        
        // Assert
        Assert.Equal(10m, OrderCreate.DiscountPercent);
        Assert.Equal(5.85m, OrderCreate.TotalAmount);
    }
    
    [Fact]
    public void Calculate_ShouldApplyNoDiscount_WhenHasOnlySandwich()
    {
        // Arrange and Act
        var OrderCreate = Order.Create([MenuItemCategory.XEgg]);
        
        // Assert
        Assert.Equal(0m, OrderCreate.DiscountPercent);
        Assert.Equal(4.50m, OrderCreate.TotalAmount);
    }
    
    [Fact]
    public void Calculate_ShouldApplyNoDiscount_WhenHasOnlyFries()
    {
        // Arrange and Act
        var OrderCreate = Order.Create([MenuItemCategory.Fries]);
        
        // Assert
        Assert.Equal(0m, OrderCreate.DiscountPercent);
        Assert.Equal(2.00m, OrderCreate.TotalAmount);
    }
    
    [Fact]
    public void Calculate_ShouldApplyNoDiscount_WhenHasOnlySoftDrink()
    {
        // Arrange and Act
        var OrderCreate = Order.Create([MenuItemCategory.SoftDrink]);
        
        // Assert
        Assert.Equal(0m, OrderCreate.DiscountPercent);
        Assert.Equal(2.50m, OrderCreate.TotalAmount);
    }
}