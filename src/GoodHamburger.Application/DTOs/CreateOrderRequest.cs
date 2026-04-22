using GoodHamburger.Domain;

namespace GoodHamburger.Application.DTOs;

public record CreateOrderRequest(IReadOnlyList<MenuItemCategory> Items);