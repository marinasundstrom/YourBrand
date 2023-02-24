using MediatR;

using Microsoft.EntityFrameworkCore;

using OrderPriceCalculator;

using YourBrand.Orders.Infrastructure.Persistence;

namespace YourBrand.Orders.Application.Orders;

public record RemoveCustomFieldFromOrderCommand(int OrderNo, string CustomFieldId) : IRequest
{
    public class RemoveCustomFieldFromOrderCommandHandler : IRequestHandler<RemoveCustomFieldFromOrderCommand>
    {
        private readonly ILogger<RemoveCustomFieldFromOrderCommandHandler> _logger;
        private readonly OrdersContext context;
 
        public RemoveCustomFieldFromOrderCommandHandler(
            ILogger<RemoveCustomFieldFromOrderCommandHandler> logger,
            OrdersContext context)
        {
            _logger = logger;
            this.context = context;
        }

        public async Task Handle(RemoveCustomFieldFromOrderCommand request, CancellationToken cancellationToken)
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

            var customField = order.CustomFields.FirstOrDefault(x => x.CustomFieldId == message.CustomFieldId);

            if (customField is null)
            {
                throw new Exception();
            }

            order.CustomFields.Remove(customField);

            context.CustomFields.Remove(customField);

            order.Update();

            await context.SaveChangesAsync();

        }
    }
}
