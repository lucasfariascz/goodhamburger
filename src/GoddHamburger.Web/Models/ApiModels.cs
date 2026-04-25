namespace GoddHamburger.Web.Models;

public record MenuItemModel(int Id, string Name, decimal Price);

public record OrderItemModel(int Id, string Name, decimal Price);

public record OrderModel(
    int Id,
    List<OrderItemModel> Items,
    decimal Subtotal,
    decimal DiscountPercent,
    decimal TotalAmount,
    DateTime CreatedAt,
    DateTime UpdatedAt);

public record ProblemDetailsModel(int? Status, string? Title, string? Detail);