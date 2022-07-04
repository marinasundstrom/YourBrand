using YourBrand.Products.Client;

using MassTransit;

using Microsoft.EntityFrameworkCore;

using OrderPriceCalculator;
using YourBrand.Orders.Contracts;
using YourBrand.Orders.Domain.Entities;
using YourBrand.Orders.Infrastructure.Persistence;
using YourBrand.Orders.Domain.Events;

namespace YourBrand.Orders.Application.Orders.Commands;

public class AddOrderItemCommandHandler : IConsumer<AddOrderItemCommand>
{
    private readonly ILogger<AddOrderItemCommandHandler> _logger;
    private readonly OrdersContext context;
    private readonly IProductsClient productsClient;
    private readonly IBus bus;

    public AddOrderItemCommandHandler(
        ILogger<AddOrderItemCommandHandler> logger,
        OrdersContext context,
        IProductsClient productsClient,
        IBus bus)
    {
        _logger = logger;
        this.context = context;
        this.productsClient = productsClient;
        this.bus = bus;
    }

    public async Task Consume(ConsumeContext<AddOrderItemCommand> consumeContext)
    {
        var message = consumeContext.Message;

        var order = await context.Orders
            .Where(c => c.OrderNo == message.OrderNo)
            .IncludeAll()
            .FirstOrDefaultAsync();

        if (order is null)
        {
            order = new Order();
            context.Orders.Add(order);
        }

        ApiProduct? product = null;

        if (message.ItemId is not null)
        {
            product = await productsClient.GetProductAsync(message.ItemId);
        }

        var orderItem = new OrderItem()
        {
            Id = Guid.NewGuid(),
            Order = order,
            Description = message.Description ?? product!.Name,
            ItemId = message.ItemId!,
            //Unit = message.Unit ?? product!.Unit.Code,
            Quantity = message.Quantity,
            Price = product.Price.GetValueOrDefault(), // product!.VatIncluded ? product.Price : product.Price.AddVat(product.VatRate),
            VatRate = 0.25 //product.VatRate
        };

        order.Items.Add(orderItem);
        context.OrderItems.Add(orderItem);

        order.Update();

        await context.SaveChangesAsync();

        await bus.Publish(new OrderItemAddedEvent(order.OrderNo, orderItem.Id));

        await consumeContext.RespondAsync<OrderItemDto>(Mappings.CreateOrderItemDto(orderItem));
    }
}
