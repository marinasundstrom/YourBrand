using MediatR;

using OrderPriceCalculator;
using YourBrand.Orders.Contracts;
using YourBrand.Orders.Infrastructure.Persistence;
using YourBrand.Orders.Domain.Events;

namespace YourBrand.Orders.Application.Orders.Commands;

public record RemoveOrderItemCommand(int OrderNo, Guid OrderItemId) : IRequest
{
    public class RemoveOrderItemCommandHandler : IRequestHandler<RemoveOrderItemCommand>
    {
        private readonly ILogger<RemoveOrderItemCommandHandler> _logger;
        private readonly OrdersContext context;
 
        public RemoveOrderItemCommandHandler(
            ILogger<RemoveOrderItemCommandHandler> logger,
            OrdersContext context)
        {
            _logger = logger;
            this.context = context;
        }

        public async Task<Unit> Handle(RemoveOrderItemCommand request, CancellationToken cancellationToken)
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

            item.Clear();

            order.RemoveItem(item);

            order.Update();

            await context.SaveChangesAsync();

            item.AddDomainEvent(new OrderItemRemovedEvent(order.OrderNo, item.Id));

            return Unit.Value;
        }
    }
}