using MediatR;

using Microsoft.EntityFrameworkCore;

using OrderPriceCalculator;

using YourBrand.Orders.Domain.Entities;
using YourBrand.Orders.Infrastructure.Persistence;

namespace YourBrand.Orders.Application.Orders;

public class AddCustomFieldToOrderCommand : IRequest
{
    public AddCustomFieldToOrderCommand(int orderNo, CreateCustomFieldDetails customFieldDetails)
    {
        OrderNo = orderNo;
        CreateCustomFieldDetails = customFieldDetails;
    }

    public int OrderNo { get; }

    public CreateCustomFieldDetails CreateCustomFieldDetails { get; }

    public class AddCustomFieldToOrderCommandHandler : IRequestHandler<AddCustomFieldToOrderCommand>
    {
        private readonly ILogger<AddCustomFieldToOrderCommandHandler> _logger;
        private readonly OrdersContext context;
 
        public AddCustomFieldToOrderCommandHandler(
            ILogger<AddCustomFieldToOrderCommandHandler> logger,
            OrdersContext context)
        {
            _logger = logger;
            this.context = context;
        }

        public async Task<Unit> Handle(AddCustomFieldToOrderCommand request, CancellationToken cancellationToken)
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

            var customField = new CustomField
            {
                Id = Guid.NewGuid(),
                CustomFieldId = details.CustomFieldId,
                Value = details.Value
            };

            order.CustomFields.Add(customField);

            context.CustomFields.Add(customField);

            order.Update();

            await context.SaveChangesAsync();

            return Unit.Value;
        }
    }
}
