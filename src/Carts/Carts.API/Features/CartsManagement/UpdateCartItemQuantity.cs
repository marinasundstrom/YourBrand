using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Carts.Domain.Entities;
using YourBrand.Carts.Persistence;

namespace YourBrand.Carts.Features.CartsManagement.Requests;

public sealed record UpdateCartItemQuantity(string CartId, string CartItemId, int Quantity) : IRequest<Result<CartItem>>
{
    public sealed class Handler(CartsContext cartsContext) : IRequestHandler<UpdateCartItemQuantity, Result<CartItem>>
    {
        public async Task<Result<CartItem>> Handle(UpdateCartItemQuantity request, CancellationToken cancellationToken)
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

            cart.UpdateCartItemQuantity(request.CartItemId, request.Quantity);

            await cartsContext.SaveChangesAsync(cancellationToken);

            return Result.Success(cartItem);
        }
    }
}