using MediatR;

using Microsoft.EntityFrameworkCore;

using OrderPriceCalculator;

using YourBrand.Orders.Domain.Entities;
using YourBrand.Orders.Infrastructure.Persistence;

namespace YourBrand.Orders.Application.Orders;

public record AddCustomFieldToOrderItemCommand(int OrderNo, Guid OrderItemId, CreateCustomFieldDetails CreateCustomFieldDetails) : IRequest
{
    public class AddCustomFieldToOrderItemCommandHandler : IRequestHandler<AddCustomFieldToOrderItemCommand>
    {
        private readonly ILogger<AddCustomFieldToOrderItemCommandHandler> _logger;
        private readonly OrdersContext context;
 
        public AddCustomFieldToOrderItemCommandHandler(
            ILogger<AddCustomFieldToOrderItemCommandHandler> logger,
            OrdersContext context)
        {
            _logger = logger;
            this.context = context;
        }

        public async Task<Unit> Handle(AddCustomFieldToOrderItemCommand request, CancellationToken cancellationToken)
        {
            var message = request;

            var details = message.CreateCustomFieldDetails;

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

            var customField = new CustomField
            {
                Id = Guid.NewGuid(),
                CustomFieldId = details.CustomFieldId,
                Value = details.Value
            };

            orderItem.CustomFields.Add(customField);

            context.CustomFields.Add(customField);

            order.Update();

            await context.SaveChangesAsync();

            return Unit.Value;
        }
    }
}
