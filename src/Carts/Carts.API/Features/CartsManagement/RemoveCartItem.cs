using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Carts.API.Domain.Entities;
using YourBrand.Carts.API.Persistence;

namespace YourBrand.Carts.API.Features.CartsManagement.Requests;

public sealed record RemoveCartItem(string CartId, string CartItemId) : IRequest<Result>
{
    public sealed class Handler(CartsContext cartsContext) : IRequestHandler<RemoveCartItem, Result>
    {
        public async Task<Result> Handle(RemoveCartItem request, CancellationToken cancellationToken)
        {
            var cart = await cartsContext.Carts
                .Include(cart => cart.Items)
                .FirstOrDefaultAsync(cart => cart.Id == request.CartId, cancellationToken);

            if (cart is null)
            {
                return Result.Failure<CartItem>(Errors.CartNotFound);
            }

            cart.RemoveItem(request.CartItemId!);

            var date = DateTimeOffset.UtcNow;
            cart.Updated = date;

            await cartsContext.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}