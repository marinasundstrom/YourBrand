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
    public AddOrderItemCommand(int orderNo, string? description, string? itemId, string? unit, decimal price, double quantity)
    {
        OrderNo = orderNo;
        Description = description;
        ItemId = itemId;
        Unit = unit;
        Price = price;
        Quantity = quantity;
    }

    public int OrderNo { get; }

    public string? Description { get; }

    public string? ItemId { get; }

    public string? Unit { get; }
    
    public decimal Price { get; }

    public double Quantity { get; }

    public class AddOrderItemCommandHandler : IRequestHandler<AddOrderItemCommand, OrderItemDto>
    {
        private readonly ILogger<AddOrderItemCommandHandler> _logger;
        private readonly OrdersContext context;
 
        public AddOrderItemCommandHandler(
            ILogger<AddOrderItemCommandHandler> logger,
            OrdersContext context)
        {
            _logger = logger;
            this.context = context;
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

            var orderItem = new OrderItem()
            {
                Id = Guid.NewGuid(),
                Order = order,
                Description = message.Description,
                ItemId = message.ItemId!,
                //Unit = message.Unit ?? product!.Unit.Code,
                Quantity = message.Quantity,
                Price = message.Price, // product!.VatIncluded ? product.Price : product.Price.AddVat(product.VatRate),
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