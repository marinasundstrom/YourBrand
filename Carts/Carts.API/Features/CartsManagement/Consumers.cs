using YourBrand.Carts.Contracts;

using MassTransit;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace YourBrand.Carts.API.Features.CartsManagement.Consumers;

public sealed class GetCartsConsumer(IMediator mediator) : IConsumer<GetCarts>
{
    public async Task Consume(ConsumeContext<GetCarts> context)
    {
        var page = context.Message.Page;
        var pageSize = context.Message.PageSize;

        var r = await mediator.Send(new Requests.GetCarts(page, pageSize), context.CancellationToken);
        var cartsResult = r.GetValue();

        var result = new PagedCartResult
        {
            Items = cartsResult.Items.Select(x => x.Map()),
            Total = cartsResult.Total
        };

        await context.RespondAsync(result);
    }
}

public sealed class GetCartByIdConsumer(IMediator mediator) : IConsumer<GetCartById>
{
    public async Task Consume(ConsumeContext<GetCartById> context)
    {
        var id = context.Message.Id;

        var r = await mediator.Send(new Requests.GetCartById(id), context.CancellationToken);
        var cart = r.GetValue();

        await context.RespondAsync(new GetCartByIdResponse { Cart = cart.Map() });
    }
}

public sealed class GetCartByTagConsumer(IMediator mediator) : IConsumer<GetCartByTag>
{
    public async Task Consume(ConsumeContext<GetCartByTag> context)
    {
        var id = context.Message.Tag;

        var r = await mediator.Send(new Requests.GetCartByTag(id), context.CancellationToken);
        var cart = r.GetValue();

        await context.RespondAsync(new GetCartByTagResponse { Cart = cart.Map() });
    }
}

public sealed class CreateCartConsumer(IMediator mediator) : IConsumer<CreateCart>
{
    public async Task Consume(ConsumeContext<CreateCart> context)
    {
        var request = context.Message;

        var r = await mediator.Send(new Requests.CreateCart(request.Tag), context.CancellationToken);

        var cart = r.GetValue();

        await context.RespondAsync(new CreateCartResponse { Cart = cart.Map() });
    }
}

public sealed class AddCartItemConsumer(IMediator mediator) : IConsumer<AddCartItem>
{
    public async Task Consume(ConsumeContext<AddCartItem> context)
    {
        var request = context.Message;

        var r = await mediator.Send(new Requests.AddCartItem(
            request.CartId,
            request.Name,
            request.Image,
            request.ProductId,
            request.ProductHandle,
            request.Description,
            request.Price,
            request.VatRate,
            request.RegularPrice,
            request.DiscountRate,
            request.Quantity,
            request.Data
        ), context.CancellationToken);

        var cartItem = r.GetValue();

        await context.RespondAsync(new AddCartItemResponse { CartItem = cartItem.Map() });
    }
}

public sealed class UpdateCartItemPriceConsumer(IMediator mediator) : IConsumer<UpdateCartItemPrice>
{
    public async Task Consume(ConsumeContext<UpdateCartItemPrice> context)
    {
        var cartId = context.Message.CartId;
        var cartItemId = context.Message.CartItemId;
        var price = context.Message.Price;

        var r = await mediator.Send(new Requests.UpdateCartItemPrice(cartId, cartItemId, price), context.CancellationToken);
        var cartItem = r.GetValue();

        await context.RespondAsync(new UpdateCartItemPriceResponse { CartItem = cartItem.Map() });
    }
}

public sealed class UpdateCartItemQuantityConsumer(IMediator mediator) : IConsumer<UpdateCartItemQuantity>
{
    public async Task Consume(ConsumeContext<UpdateCartItemQuantity> context)
    {
        var cartId = context.Message.CartId;
        var cartItemId = context.Message.CartItemId;
        var quantity = context.Message.Quantity;

        var r = await mediator.Send(new Requests.UpdateCartItemQuantity(cartId, cartItemId, quantity), context.CancellationToken);
        var cartItem = r.GetValue();

        await context.RespondAsync(new UpdateCartItemQuantityResponse { CartItem = cartItem.Map() });
    }
}

public sealed class UpdateCartItemDataConsumer(IMediator mediator) : IConsumer<UpdateCartItemData>
{
    public async Task Consume(ConsumeContext<UpdateCartItemData> context)
    {
        var cartId = context.Message.CartId;
        var cartItemId = context.Message.CartItemId;
        var data = context.Message.Data;

        var r = await mediator.Send(new Requests.UpdateCartItemData(cartId, cartItemId, data), context.CancellationToken);
        var cartItem = r.GetValue();

        await context.RespondAsync(new UpdateCartItemDataResponse { CartItem = cartItem.Map() });
    }
}

public sealed class RemoveCartItemQuantityConsumer(IMediator mediator) : IConsumer<RemoveCartItem>
{
    public async Task Consume(ConsumeContext<RemoveCartItem> context)
    {
        var cartId = context.Message.CartId;
        var cartItemId = context.Message.CartItemId;

        var r = await mediator.Send(new Requests.RemoveCartItem(cartId, cartItemId), context.CancellationToken);

        await context.RespondAsync(new RemoveCartItemResponse());
    }
}

public sealed class ClearCartConsumer(IMediator mediator) : IConsumer<ClearCart>
{
    public async Task Consume(ConsumeContext<ClearCart> context)
    {
        var cartId = context.Message.CartId;

        var r = await mediator.Send(new Requests.ClearCart(cartId), context.CancellationToken);

        await context.RespondAsync(new ClearCartResponse());
    }
}

public static class Mappings
{
    public static Cart Map(this Domain.Entities.Cart cart) => new()
    {
        Id = cart.Id,
        Tag = cart.Tag,
        Total = cart.Total,
        Items = cart.Items.Select(cartItem => cartItem.Map())
    };

    public static CartItem Map(this Domain.Entities.CartItem cartItem) => new()
    {
        Id = cartItem.Id,
        Name = cartItem.Name,
        Image = cartItem.Image,
        ProductId = cartItem.ProductId,
        ProductHandle = cartItem.ProductHandle,
        Description = cartItem.Description,
        Price = cartItem.Price,
        VatRate = cartItem.VatRate,
        RegularPrice = cartItem.RegularPrice,
        DiscountRate = cartItem.DiscountRate,
        Quantity = cartItem.Quantity,
        Total = cartItem.Total,
        Data = cartItem.Data,
        Created = cartItem.Created,
        Updated = cartItem.Updated
    };
}

public sealed class ProductPriceUpdatedConsumer(Persistence.CartsContext cartsContext) : IConsumer<Catalog.Contracts.ProductPriceUpdated>
{
    public async Task Consume(ConsumeContext<Catalog.Contracts.ProductPriceUpdated> context)
    {
        var message = context.Message;

        await cartsContext.CartItems
            .Where(cartItem => cartItem.ProductId == message.ProductId)
            .ExecuteUpdateAsync(s =>
                s.SetProperty(e => e.Price, e => message.NewPrice)
                .SetProperty(e => e.RegularPrice, e => message.RegularPrice)
                .SetProperty(e => e.DiscountRate, message.DiscountRate), context.CancellationToken);

    }
}

public sealed class ProductVatRateUpdatedConsumer(Persistence.CartsContext cartsContext) : IConsumer<Catalog.Contracts.ProductVatRateUpdated>
{
    public async Task Consume(ConsumeContext<Catalog.Contracts.ProductVatRateUpdated> context)
    {
        var message = context.Message;

        await cartsContext.CartItems
            .Where(cartItem => cartItem.ProductId == message.ProductId)
            .ExecuteUpdateAsync(s =>
                s.SetProperty(e => e.VatRate, e => message.NewVatRate), context.CancellationToken);

    }
}

public sealed class ProductDetailsUpdatedConsumer(Persistence.CartsContext cartsContext) : IConsumer<Catalog.Contracts.ProductDetailsUpdated>
{
    public async Task Consume(ConsumeContext<Catalog.Contracts.ProductDetailsUpdated> context)
    {
        var message = context.Message;

        await cartsContext.CartItems
            .Where(cartItem => cartItem.ProductId == message.ProductId)
            .ExecuteUpdateAsync(s =>
                s.SetProperty(e => e.Name, e => message.Name)
                 .SetProperty(e => e.Description, e => message.Description), context.CancellationToken);
    }
}

public sealed class ProductImageUpdatedConsumer(Persistence.CartsContext cartsContext) : IConsumer<Catalog.Contracts.ProductImageUpdated>
{
    public async Task Consume(ConsumeContext<Catalog.Contracts.ProductImageUpdated> context)
    {
        var message = context.Message;

        await cartsContext.CartItems
            .Where(cartItem => cartItem.ProductId == message.ProductId)
            .ExecuteUpdateAsync(s => s.SetProperty(e => e.Image, e => message.ImageUrl), context.CancellationToken);
    }
}


public sealed class ProductHandleUpdatedConsumer(Persistence.CartsContext cartsContext) : IConsumer<Catalog.Contracts.ProductHandleUpdated>
{
    public async Task Consume(ConsumeContext<Catalog.Contracts.ProductHandleUpdated> context)
    {
        var message = context.Message;

        await cartsContext.CartItems
            .Where(cartItem => cartItem.ProductId == message.ProductId)
            .ExecuteUpdateAsync(s => s.SetProperty(e => e.ProductHandle, e => message.Handle), context.CancellationToken);
    }
}