using MediatR;

using Microsoft.EntityFrameworkCore;

using OrderPriceCalculator;

using YourBrand.Orders.Infrastructure.Persistence;

namespace YourBrand.Orders.Application.Orders;

public record RemoveCustomFieldFromOrderItemCommand(int OrderNo, Guid OrderItemId, string CustomFieldId) : IRequest
{
    public class RemoveCustomFieldFromOrderItemCommandHandler : IRequestHandler<RemoveCustomFieldFromOrderItemCommand>
    {
        private readonly ILogger<RemoveCustomFieldFromOrderItemCommandHandler> _logger;
        private readonly OrdersContext context;
 
        public RemoveCustomFieldFromOrderItemCommandHandler(
            ILogger<RemoveCustomFieldFromOrderItemCommandHandler> logger,
            OrdersContext context)
        {
            _logger = logger;
            this.context = context;
        }

        public async Task Handle(RemoveCustomFieldFromOrderItemCommand request, CancellationToken cancellationToken)
        {
            var message = request;

            var order = await context.Orders
             .IncludeAll()
             .Where(c => c.OrderNo == message.OrderNo)
             .FirstOrDefaultAsync();

            if (order is null)
            {
                throw new Exception();
            }

            var orderItem = order.Items.FirstOrDefault(i => i.Id == message.OrderItemId);

            if (orderItem is null)
            {
                throw new Exception();
            }

            var customField = orderItem.CustomFields.FirstOrDefault(x => x.CustomFieldId == message.CustomFieldId);

            if (customField is null)
            {
                throw new Exception();
            }

            orderItem.CustomFields.Remove(customField);

            context.CustomFields.Remove(customField);

            order.Update();

            await context.SaveChangesAsync();

        }
    }
}