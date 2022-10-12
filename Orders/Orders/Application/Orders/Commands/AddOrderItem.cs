using YourBrand.Catalog.Client;

using MediatR;

using Microsoft.EntityFrameworkCore;

using OrderPriceCalculator;
using YourBrand.Orders.Contracts;
using YourBrand.Orders.Domain.Entities;
using YourBrand.Orders.Infrastructure.Persistence;
using YourBrand.Orders.Domain.Events;

namespace YourBrand.Orders.Application.Orders.Commands;

public class AddOrderItemCommand : IRequest<OrderItemDto>
{
    public AddOrderItemCommand(int orderNo, string? description, string? itemId, string? unit, double quantity)
    {
        OrderNo = orderNo;
        Description = description;
        ItemId = itemId;
        Unit = unit;
        Quantity = quantity;
    }

    public int OrderNo { get; }

    public string? Description { get; }

    public string? ItemId { get; }

    public string? Unit { get; }

    public double Quantity { get; }

    public class AddOrderItemCommandHandler : IRequestHandler<AddOrderItemCommand, OrderItemDto>
    {
        private readonly ILogger<AddOrderItemCommandHandler> _logger;
        private readonly OrdersContext context;
        private readonly IProductsClient productsClient;
 
        public AddOrderItemCommandHandler(
            ILogger<AddOrderItemCommandHandler> logger,
            OrdersContext context,
            IProductsClient productsClient)
        {
            _logger = logger;
            this.context = context;
            this.productsClient = productsClient;
        }

        public async Task<OrderItemDto> Handle(AddOrderItemCommand request, CancellationToken cancellationToken)
        {
            var message = request;

            var order = await context.Orders
                .Where(c => c.OrderNo == message.OrderNo)
                .IncludeAll()
                .FirstOrDefaultAsync();

            if (order is null)
            {
                order = new Order();
                context.Orders.Add(order);
            }

            ProductDto? product = null;

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

            //await bus.Publish(new OrderItemAddedEvent(order.OrderNo, orderItem.Id));

            return Mappings.CreateOrderItemDto(orderItem);
        }
    }
}