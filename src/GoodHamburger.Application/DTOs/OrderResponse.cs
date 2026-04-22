namespace GoodHamburger.Application.DTOs;

public record OrderResponse(
    int Id,
    IReadOnlyList<OrderItemResponse> Items,
    decimal Subtotal,
    decimal DiscountPercent,
    decimal TotalAmount,
    DateTime CreatedAt,
    DateTime UpdatedAt);