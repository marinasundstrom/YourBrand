using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Carts.Domain.Entities;
using YourBrand.Carts.Persistence;

namespace YourBrand.Carts.Features.CartsManagement.Requests;

public sealed record UpdateCartItemPrice(string CartId, string CartItemId, decimal Price) : IRequest<Result<CartItem>>
{
    public sealed class Handler(CartsContext cartsContext) : IRequestHandler<UpdateCartItemPrice, Result<CartItem>>
    {
        public async Task<Result<CartItem>> Handle(UpdateCartItemPrice request, CancellationToken cancellationToken)
        {
            var cart = await cartsContext.Carts
                .Include(cart => cart.Items)
                .FirstOrDefaultAsync(cart => cart.Id == request.CartId, cancellationToken);

            if (cart is null)
            {
                return Result.Failure<CartItem>(Errors.CartNotFound);
            }

            var cartItem = cart.Items.FirstOrDefault(x => x.Id == request.CartItemId!);

            if (cartItem is null)
            {
                return Result.Failure<CartItem>(Errors.CartItemNotFound);
            }

            cart.UpdateCartItemPrice(request.CartItemId, request.Price);

            await cartsContext.SaveChangesAsync(cancellationToken);

            return Result.SuccessWith(cartItem);
        }
    }
}