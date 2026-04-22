using GoodHamburger.Domain.Exceptions;

namespace GoodHamburger.Domain.Entities;

public class Order
{
    public int Id { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime UpdatedAt { get; set; }

    public decimal TotalAmount { get; set; }
    
    public decimal DiscountPercent { get; set; }
    
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
    
    public void Update(IEnumerable<MenuItemCategory> items)
    {
        _items.Clear();
        SetItems(items);
        UpdatedAt = DateTime.UtcNow;
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

        RecalculateTotals();
    }

    private void RecalculateTotals()
    {
        var SubTotal = _items.Sum(i => GetPrice(i.Item));
        var HasSandwich = _items.Any(i => Sandwiches.Contains(i.Item));
        var HasFries = _items.Any(i => i.Item == MenuItemCategory.Fries);
        var HasSoftDrink = _items.Any(i => i.Item == MenuItemCategory.SoftDrink);

        if (HasSandwich && HasFries && HasSoftDrink)
        {
            DiscountPercent = 20m;
        }
        else if (HasSandwich && HasSoftDrink)
        {
            DiscountPercent = 15m;
        }
        else if (HasSandwich && HasFries)
        {
            DiscountPercent = 10m;
        }
        else
        {
            DiscountPercent = 0m;
        }

        TotalAmount = Math.Round(SubTotal * (1 - DiscountPercent / 100m), 2, MidpointRounding.AwayFromZero);
    }

    public static decimal GetPrice(MenuItemCategory Category)
    {
        switch (Category)
        {
            case MenuItemCategory.XBurger:
                return 5.00m;
            case MenuItemCategory.XEgg:
                return 4.50m;
            case MenuItemCategory.XBacon:
                return 7.00m;
            case MenuItemCategory.Fries: 
                return 2.00m;
            case MenuItemCategory.SoftDrink:
                return 2.50m;
            default:
                throw new DomainException("Unknown category");
        }
    }
}