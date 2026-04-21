namespace GoodHamburger.Domain.Entities;

public class OrderItem
{
    public int Id { get; set; }
    
    public int OrderId { get; set; }
    
    public MenuItemCategory Category { get; set; }

    public OrderItem(MenuItemCategory category)
    {
        Category = category;
    }
}