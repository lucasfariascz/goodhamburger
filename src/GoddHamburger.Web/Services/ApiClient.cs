using GoddHamburger.Web.Components.Pages;
using GoddHamburger.Web.Models;

namespace GoddHamburger.Web.Services;

public class ApiClient(HttpClient Http)
{
    public async Task<List<MenuItemModel>> GetMenuAsync()
    {
        return await Http.GetFromJsonAsync<List<MenuItemModel>>("/api/menuItem") ?? [];
    }

    public async Task<List<OrderModel>> GetOrdersAsync()
    {
        return await Http.GetFromJsonAsync<List<OrderModel>>("/api/orders") ?? [];
    }

    public async Task<(OrderModel? Result, string? Error)> CreateOrderAsync(IEnumerable<int> items)
    {
        var response = await Http.PostAsJsonAsync("/api/orders", new { items });

        if (response.IsSuccessStatusCode)
            return (await response.Content.ReadFromJsonAsync<OrderModel>(), null);

        var problem = await response.Content.ReadFromJsonAsync<ProblemDetailsModel>();
        return (null, problem?.Detail ?? "Erro ao criar pedido.");
    }
}