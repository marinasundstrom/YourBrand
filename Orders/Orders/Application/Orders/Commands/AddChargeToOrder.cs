using MediatR;

using Microsoft.EntityFrameworkCore;

using OrderPriceCalculator;

using YourBrand.Orders.Contracts;
using YourBrand.Orders.Domain.Entities;
using YourBrand.Orders.Infrastructure.Persistence;

namespace YourBrand.Orders.Application.Orders.Commands;

public record AddChargeToOrderCommand(int OrderNo, ChargeDetails ChargeDetails) : IRequest
{
    public class AddChargeToOrderCommandHandler : IRequestHandler<AddChargeToOrderCommand>
    {
        private readonly ILogger<AddChargeToOrderCommandHandler> _logger;
        private readonly OrdersContext context;
 
        public AddChargeToOrderCommandHandler(
            ILogger<AddChargeToOrderCommandHandler> logger,
            OrdersContext context)
        {
            _logger = logger;
            this.context = context;
        }

        public async Task Handle(AddChargeToOrderCommand request, CancellationToken cancellationToken)
        {
            var message = request;

            var details = message.ChargeDetails;

            var order = await context.Orders
                 .IncludeAll()
                 .Where(c => c.OrderNo == message.OrderNo)
                 .FirstOrDefaultAsync();

            if (order is null)
            {
                throw new Exception();
            }

            if (details.Percent is not null)
            {
                if (order.Charges.Any(x => x.Percent is null))
                {
                    throw new Exception("Cannot combine different Charge types.");
                }

                if (order.Charges.Any(x => x.Percent is not null))
                {
                    throw new Exception("Cannot add another Charge based on percenOrderNoe.");
                }
            }

            if (details.Percent is null && order.Charges.Any(x => x.Percent is not null))
            {
                throw new Exception("Cannot combine different Charge types.");
            }

            var Charge = new OrderCharge
            {
                Id = Guid.NewGuid(),
                Order = order,
                Amount = details.Amount,
                Percent = details.Percent,
                Description = details.Description!,
                ChargeId = details.ChargeId
            };

            order.Charges.Add(Charge);

            context.OrderCharges.Add(Charge);

            order.Update();

            await context.SaveChangesAsync();

        }
    }
}
