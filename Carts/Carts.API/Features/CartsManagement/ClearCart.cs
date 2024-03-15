using YourBrand.Carts.API.Domain.Entities;
using YourBrand.Carts.API.Persistence;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace YourBrand.Carts.API.Features.CartsManagement.Requests;

public sealed record ClearCart(string CartId) : IRequest<Result>
{
    public sealed class Handler(CartsContext cartsContext) : IRequestHandler<ClearCart, Result>
    {
        public async Task<Result> Handle(ClearCart request, CancellationToken cancellationToken)
        {
            var cart = await cartsContext.Carts
                .Include(cart => cart.Items)
                .FirstOrDefaultAsync(cart => cart.Id == request.CartId, cancellationToken);

            if (cart is null)
            {
                return Result.Failure<CartItem>(Errors.CartNotFound);
            }

            cart.Clear();

            await cartsContext.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
