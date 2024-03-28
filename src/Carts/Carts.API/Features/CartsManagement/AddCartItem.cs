using YourBrand.Carts.API.Domain.Entities;
using YourBrand.Carts.API.Persistence;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace YourBrand.Carts.API.Features.CartsManagement.Requests;

public sealed record AddCartItem(string CartId, string Name, string? Image, long? ProductId, string? ProductHandle, string Description, decimal Price, double? VatRate, decimal? RegularPrice, double? DiscountRate, int Quantity, string? Data) : IRequest<Result<CartItem>>
{
    public sealed class Handler(CartsContext cartsContext) : IRequestHandler<AddCartItem, Result<CartItem>>
    {
        public async Task<Result<CartItem>> Handle(AddCartItem request, CancellationToken cancellationToken)
        {
            var cart = await cartsContext.Carts
                .Include(cart => cart.Items)
                .FirstOrDefaultAsync(cart => cart.Id == request.CartId, cancellationToken);

            if (cart is null)
            {
                return Result.Failure<CartItem>(Errors.CartNotFound);
            }

            var cartItem = cart.AddItem(request.Name, request.Image, request.ProductId, request.ProductHandle, request.Description, request.Price, request.VatRate, request.RegularPrice, request.DiscountRate, request.Quantity, request.Data);

            var date = DateTimeOffset.UtcNow;
            cartItem.Created = date;
            cart.Updated = date;

            await cartsContext.SaveChangesAsync(cancellationToken);

            return Result.Success(cartItem);
        }
    }
}
