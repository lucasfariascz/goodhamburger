namespace GoodHamburger.Domain.Entities;

public class OrderItem
{
    public int Id { get; set; }
    
    public int OrderId { get; set; }
    
    public MenuItemCategory Item { get; set; }

    public OrderItem(MenuItemCategory item)
    {
        Item = item;
    }
}