using BlazorApp.BankId;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using YourBrand.StoreFront;

namespace BlazorApp.Cart;

[ApiController]
[Route("api/Cart")]
public sealed class CartController : ControllerBase
{
    [HttpGet]
    [ProducesDefaultResponseType]
    [ProducesResponseType<YourBrand.StoreFront.Cart>(StatusCodes.Status200OK)]
    public async Task<ActionResult<YourBrand.StoreFront.Cart>> GetCart([FromServices] ICartClient cartClient = default!, CancellationToken cancellationToken = default)
    {
        var cart = await cartClient.GetCartAsync(cancellationToken);

        return Ok(cart);
    }


    [HttpPost("Items")]
    [ProducesDefaultResponseType]
    [ProducesResponseType<YourBrand.StoreFront.CartItem>(StatusCodes.Status200OK)]
    public async Task<ActionResult<YourBrand.StoreFront.CartItem>> AddCartItem(AddCartItemRequest request, [FromServices] ICartClient cartClient = default!, CancellationToken cancellationToken = default)
    {
        var cartItem = await cartClient.AddCartItemAsync(request, cancellationToken);

        return Ok(cartItem);
    }

    [HttpPut("Items/{cartItemId}/Quantity")]
    [ProducesDefaultResponseType]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<YourBrand.StoreFront.CartItem>> UpdateCartItemQuantity(string cartItemId, UpdateCartItemQuantityRequest request, [FromServices] ICartClient cartClient = default!, CancellationToken cancellationToken = default)
    {
        var cartItem = await cartClient.UpdateCartItemQuantityAsync(cartItemId, request, cancellationToken);

        return Ok(cartItem);
    }

    [HttpDelete("Items/{cartItemId}")]
    [ProducesDefaultResponseType]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> RemoveCartItem(string cartItemId, [FromServices] ICartClient cartClient = default!, CancellationToken cancellationToken = default)
    {
        await cartClient.RemoveCartItemAsync(cartItemId, cancellationToken);
        
        return Ok();
    }

    [HttpPut("Items/{cartItemId}/Data")]
    [ProducesDefaultResponseType]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> UpdateCartItemData(string cartItemId, UpdateCartItemDataRequest request, [FromServices] ICartClient cartClient = default!, CancellationToken cancellationToken = default)
    {
        await cartClient.UpdateCartItemDataAsync(cartItemId, request);

        return Ok();
    }

    [HttpDelete]
    [ProducesDefaultResponseType]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> ClearCart([FromServices] ICartClient cartClient = default!, CancellationToken cancellationToken = default)
    {
        await cartClient.ClearCartAsync(cancellationToken);

        return Ok();
    }
}