using GoodHamburger.Domain;

namespace GoodHamburger.Application.DTOs;

public record UpdateOrderRequest(IReadOnlyList<MenuItemCategory> Items);
