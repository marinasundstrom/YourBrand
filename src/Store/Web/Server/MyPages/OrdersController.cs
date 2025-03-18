using BlazorApp.BankId;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using YourBrand.StoreFront;

namespace BlazorApp.MyPages;

[ApiController]
[Authorize]
[Route("Orders")]
public sealed class OrdersController : ControllerBase
{
    [HttpGet]
    [ProducesDefaultResponseType]
    [ProducesResponseType<PagedResultOfOrder>(StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResultOfOrder>> GetOrders(int page = 1, int pageSize = 10, string? searchTerm = null, [FromServices] IOrdersClient ordersClient = default!, CancellationToken cancellationToken = default)
    {
        var result = await ordersClient.GetOrdersAsync(page, pageSize, searchTerm, null, cancellationToken);

        return Ok(result);
    }
}

public record OrdersStatusResponse(BankIdStatus Status, string? QrCode);