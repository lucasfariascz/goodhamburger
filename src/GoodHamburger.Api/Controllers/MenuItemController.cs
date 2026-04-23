using GoodHamburger.Application.DTOs;
using GoodHamburger.Domain;
using GoodHamburger.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace GoodHamburger.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class MenuItemController : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<MenuItemResponse>),StatusCodes.Status200OK)]
    public IActionResult GetMenuItem()
    {
        var Items = Enum.GetValues<MenuItemCategory>()
            .Select(T => new MenuItemResponse((int)T, T.ToString(), Order.GetPrice(T)))
            .ToList();

        return Ok(Items);
    }
}