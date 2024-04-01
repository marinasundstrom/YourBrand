using FluentValidation;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Carts.API.Domain.Entities;
using YourBrand.Carts.API.Persistence;

namespace YourBrand.Carts.API.Features.CartsManagement.Requests;

public sealed record UpdateCartItemData(string CartId, string CartItemId, string? Data) : IRequest<Result<CartItem>>
{
    public sealed class Validator : AbstractValidator<RemoveCartItem>
    {
        public Validator()
        {
            RuleFor(x => x.CartId).NotEmpty();

            RuleFor(x => x.CartItemId).NotEmpty();
        }
    }

    public sealed class Handler(CartsContext cartsContext) : IRequestHandler<UpdateCartItemData, Result<CartItem>>
    {
        public async Task<Result<CartItem>> Handle(UpdateCartItemData request, CancellationToken cancellationToken)
        {
            var cart = await cartsContext.Carts
                .Include(x => x.Items)
                .FirstOrDefaultAsync(x => x.Id == request.CartId, cancellationToken);

            if (cart is null)
            {
                return Result.Failure<CartItem>(Errors.CartNotFound);
            }

            var cartItem = cart.Items.FirstOrDefault(x => x.Id == request.CartItemId);

            if (cartItem is null)
            {
                throw new System.Exception();
            }

            cartItem.UpdateData(request.Data);

            var date = DateTimeOffset.UtcNow;
            cartItem.Updated = date;
            cart.Updated = date;

            await cartsContext.SaveChangesAsync(cancellationToken);

            return Result.Success(cartItem);
        }
    }
}