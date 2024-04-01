using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Carts.API.Domain.Entities;
using YourBrand.Carts.API.Persistence;

namespace YourBrand.Carts.API.Features.CartsManagement.Requests;

public sealed record GetCarts(int Page = 1, int PageSize = 2) : IRequest<Result<PagedResult<Cart>>>
{
    public sealed class Handler(CartsContext cartsContext = default!) : IRequestHandler<GetCarts, Result<PagedResult<Cart>>>
    {
        public async Task<Result<PagedResult<Cart>>> Handle(GetCarts request, CancellationToken cancellationToken)
        {
            var query = cartsContext.Carts
                .Include(cart => cart.Items.OrderBy(cartItem => cartItem.Created))
                .AsQueryable();

            var total = await query.CountAsync(cancellationToken);

            var carts = await query.OrderBy(x => x.Id)
                .Skip(request.PageSize * (request.Page - 1))
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            var pagedResult = new PagedResult<Cart>(carts, total);

            return Result.Success(pagedResult);
        }
    }
}