using MediatR;

using OrderPriceCalculator;
using YourBrand.Orders.Contracts;
using YourBrand.Orders.Infrastructure.Persistence;
using YourBrand.Orders.Domain.Events;

namespace YourBrand.Orders.Application.Orders.Commands;

public record UpdateOrderItemQuantityCommand(int OrderNo, Guid OrderItemId, double Quantity) : IRequest
{
    public class UpdateOrderItemQuantityCommandHandler : IRequestHandler<UpdateOrderItemQuantityCommand>
    {
        private readonly ILogger<UpdateOrderItemQuantityCommandHandler> _logger;
        private readonly OrdersContext context;
 
        public UpdateOrderItemQuantityCommandHandler(
            ILogger<UpdateOrderItemQuantityCommandHandler> logger,
            OrdersContext context)
        {
            _logger = logger;
            this.context = context;
        }

        public async Task Handle(UpdateOrderItemQuantityCommand request, CancellationToken cancellationToken)
        {
            var message = request;

            var order = context.Orders
                .Where(c => c.OrderNo == message.OrderNo)
                .IncludeAll()
                .FirstOrDefault();

            if (order is null)
            {
                throw new Exception();
            }

            var item = order.Items.FirstOrDefault(i => i.Id == message.OrderItemId);

            if (item is null)
            {
                throw new Exception();
            }

            var oldQuantity = item.Quantity;

            item.UpdateQuantity(message.Quantity);

            order.Update();

            await context.SaveChangesAsync();

            //await bus.Publish(new OrderItemQuantityUpdatedEvent(order.OrderNo, item.Id, oldQuantity, item.Quantity));

        }
    }
}