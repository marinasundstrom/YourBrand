using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Carts.API.Domain.Entities;
using YourBrand.Carts.API.Persistence;

namespace YourBrand.Carts.API.Features.CartsManagement.Requests;

public sealed record GetCartById(string Id) : IRequest<Result<Cart>>
{
    public sealed class Handler(CartsContext cartsContext) : IRequestHandler<GetCartById, Result<Cart>>
    {
        public async Task<Result<Cart>> Handle(GetCartById request, CancellationToken cancellationToken)
        {
            var cart = await cartsContext.Carts
                .Include(cart => cart.Items.OrderBy(cartItem => cartItem.Created))
                .FirstOrDefaultAsync(cart => cart.Id == request.Id, cancellationToken);

            return cart is not null ? Result.Success(cart) : Result.Failure<Cart>(Errors.CartNotFound);
        }
    }
}