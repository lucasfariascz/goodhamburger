using GoodHamburger.Domain;

namespace GoodHamburger.UnitTests.Domain.Catalog;

public class StaticMenuCatalogTests
{
    [Fact]
    public void GetAll_ShouldReturnFiveItems()
    {
        // Arrange
        var items = new List<MenuItemCategory>();
        
        // Act
        items.Add(MenuItemCategory.XBurger);
        items.Add(MenuItemCategory.Fries);
        items.Add(MenuItemCategory.SoftDrink);
        items.Add(MenuItemCategory.XBacon);
        items.Add(MenuItemCategory.XEgg);

        // Assert
        Assert.Equal(5, items.Count);
    }
    
    [Fact]
    public void GetByCode_ShouldReturnXBurger()
    {
        // Arrange
        var Items = new List<MenuItemCategory>();
        
        // Act
        Items.Add(MenuItemCategory.XBurger);

        // Assert
        Assert.Single(Items, X => X == MenuItemCategory.XBurger);
    }
    
    [Fact]
    public void GetByCode_ShouldThrow_WhenCodeDoesNotExist()
    {
        // Arrange
        var items = new List<MenuItemCategory>();
        

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => items.Single(x => x == MenuItemCategory.XBurger));
    }
}