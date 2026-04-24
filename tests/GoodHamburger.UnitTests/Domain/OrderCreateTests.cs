using GoodHamburger.Domain;
using GoodHamburger.Domain.Entities;
using GoodHamburger.Domain.Exceptions;

namespace GoodHamburger.UnitTests.Domain.Entities;

public class OrderCreateTests
{
    [Fact]
    public void Create_ShouldAllowSingleSandwich()
    {
        
        // Arrange and Act
        var OrderCreate = Order.Create([MenuItemCategory.XBurger]);

        // Assert
        Assert.NotNull(OrderCreate);
        Assert.Contains(OrderCreate.Items, Item => Item.Item == MenuItemCategory.XBurger);
        Assert.Single(OrderCreate.Items);
    }

    [Fact]
    public void Create_ShouldAllowSandwichAndFries()
    {
        // Arrange and Act
        var OrderCreate = Order.Create([MenuItemCategory.XBurger, MenuItemCategory.Fries]);

        // Assert
        Assert.NotNull(OrderCreate);
        Assert.Contains(OrderCreate.Items, Item => Item.Item == MenuItemCategory.XBurger);
        Assert.Contains(OrderCreate.Items, Item => Item.Item == MenuItemCategory.Fries);
        Assert.Equal(2, OrderCreate.Items.Count);
    }
    
    [Fact]
    public void Create_ShouldAllowSandwichAndFriesAndSoftDrink()
    {
        // Arrange and Act
        var OrderCreate = Order.Create([MenuItemCategory.XBurger, MenuItemCategory.Fries, MenuItemCategory.SoftDrink]);

        // Assert
        Assert.NotNull(OrderCreate);
        Assert.Contains(OrderCreate.Items, Item => Item.Item == MenuItemCategory.XBurger);
        Assert.Contains(OrderCreate.Items, Item => Item.Item == MenuItemCategory.Fries);
        Assert.Contains(OrderCreate.Items, Item => Item.Item == MenuItemCategory.SoftDrink);
        Assert.Equal(3, OrderCreate.Items.Count);
    }

    [Fact]
    public void Create_ShouldThrow_WhenTwoSandwichesAreAdded()
    {
        // Arrange and Act
        var Exception = Assert.Throws<DuplicateItemException>(() => Order.Create([MenuItemCategory.XBurger, MenuItemCategory.XBacon])); 
        
        // Assert
        Assert.IsType<DuplicateItemException>(Exception);
        Assert.Equal("Sandwiches must contain more than one item", Exception.Message);
    }

    [Fact]
    public void Create_ShouldThrow_WhenTwoSidesAreAdded()
    {
        // Arrange and Act
        var Exception = Assert.Throws<DuplicateItemException>(() => Order.Create([MenuItemCategory.Fries, MenuItemCategory.Fries]));
        
        // Assert
        Assert.IsType<DuplicateItemException>(Exception);
        Assert.Equal("Fries must contain more than one item", Exception.Message);
    }

    [Fact]
    public void Create_ShouldThrow_WhenTwoDrinksAreAdded()
    {
        // Arrange and Act
        var Exception = Assert.Throws<DuplicateItemException>(() => Order.Create([MenuItemCategory.SoftDrink, MenuItemCategory.SoftDrink]));
        
        // Assert
        Assert.IsType<DuplicateItemException>(Exception);
        Assert.Equal("SoftDrink must contain more than one item", Exception.Message);
    }

    [Fact]
    public void Create_ShouldThrow_WhenOrderHasNoItems()
    {
        // Arrange and Act
        var Exception = Assert.Throws<NotFoundException>(() => Order.Create([]));
        
        // Assert
        Assert.Equal("The order must contain at least 1 item", Exception.Message);
    }
}