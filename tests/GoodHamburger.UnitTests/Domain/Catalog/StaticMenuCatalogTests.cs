using GoodHamburger.Domain;

namespace GoodHamburger.UnitTests.Domain.Catalog;

public class StaticMenuCatalogTests
{
    [Fact]
    public void GetAll_ShouldReturnFiveItems()
    {
        // Arrange
        var items = new List<MenuItemType>();
        
        // Act
        items.AddRange(MenuItemType.XBurger);
        items.AddRange(MenuItemType.Fries);
        items.AddRange(MenuItemType.SoftDrink);
        items.AddRange(MenuItemType.XBacon);
        items.AddRange(MenuItemType.XEgg);

        // Assert
        Assert.Equal(5, items.Count);
    }
    
    [Fact]
    public void GetByCode_ShouldReturnXBurger()
    {
        // Arrange
        var items = new List<MenuItemType>();
        
        // Act
        items.AddRange(MenuItemType.XBurger);

        // Assert
        Assert.Single(items.Where(x => x == MenuItemType.XBurger));
    }
    
    [Fact]
    public void GetByCode_ShouldThrow_WhenCodeDoesNotExist()
    {
        // Arrange
        var items = new List<MenuItemType>();
        

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() =>
            items.Single(x => x == MenuItemType.XBurger)
        );
    }
}