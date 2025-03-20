using BlazorApp.BankId;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using YourBrand.StoreFront;

namespace BlazorApp.Checkout;

[ApiController]
[Route("api/Checkout")]
public sealed class CheckoutController : ControllerBase
{
    [HttpPost]
    [ProducesDefaultResponseType]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> Checkout(YourBrand.StoreFront.Checkout request, [FromServices] ICheckoutClient checkoutClient = default!, CancellationToken cancellationToken = default)
    {
        await checkoutClient.CheckoutAsync(request, cancellationToken);

        return Ok();
    }
}