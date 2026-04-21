using GoodHamburger.Domain.Exceptions;

namespace GoodHamburger.Domain.Entities;

public class Order
{
    public int Id { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime UpdatedAt { get; set; }

    public decimal TotalAmount { get; set; }
    
    public decimal TotalDiscount { get; set; }
    
    private readonly List<OrderItem> _items = [];
    
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

    private static readonly IReadOnlySet<MenuItemCategory> Sandwiches = new HashSet<MenuItemCategory>
    {
        MenuItemCategory.XBurger, MenuItemCategory.XBacon, MenuItemCategory.XEgg
    };

    public static Order Create(IEnumerable<MenuItemCategory> items)
    {
        var Order = new Order();
        Order.SetItems(items);
        return Order;
    }

    private void SetItems(IEnumerable<MenuItemCategory> items)
    {
        var ItemList = items.ToList();

        if (ItemList.Count == 0)
        {
            throw new DomainException("The order must contain at least 1 item");
        }
        
        var SandwichCount = ItemList.Count(x => Sandwiches.Contains(x));
        if (SandwichCount > 1)
        {
            throw new DomainException("Sandwiches must contain more than one item");
        }
        
        var FriesCount = ItemList.Count(x => x == MenuItemCategory.Fries);
        if (FriesCount > 1) 
        {
            throw new DomainException("Fries must contain more than one item");
        }

        var SoftDrinkCount = ItemList.Count(x => x == MenuItemCategory.SoftDrink);
        if (SoftDrinkCount > 1)
        {
            throw new DomainException("SoftDrink must contain more than one item");
        }

        foreach (var Item in ItemList)
        {
            _items.Add(new OrderItem(Item));
        }
    }
}