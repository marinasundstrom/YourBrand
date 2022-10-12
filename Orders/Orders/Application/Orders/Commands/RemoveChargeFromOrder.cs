using MediatR;

using Microsoft.EntityFrameworkCore;

using OrderPriceCalculator;

using YourBrand.Orders.Contracts;
using YourBrand.Orders.Infrastructure.Persistence;

namespace YourBrand.Orders.Application.Orders.Commands;

public class RemoveChargeFromOrderCommand : IRequest
{
    public RemoveChargeFromOrderCommand(
        int orderNo,
        Guid chargeId
    )
    {
        OrderNo = orderNo;
        ChargeId = chargeId;
    }

    public int OrderNo { get; }

    public Guid ChargeId { get; }

    public class RemoveChargeFromOrderCommandHandler : IRequestHandler<RemoveChargeFromOrderCommand>
    {
        private readonly ILogger<RemoveChargeFromOrderCommandHandler> _logger;
        private readonly OrdersContext context;
 
        public RemoveChargeFromOrderCommandHandler(
            ILogger<RemoveChargeFromOrderCommandHandler> logger,
            OrdersContext context)
        {
            _logger = logger;
            this.context = context;
        }

        public async Task<Unit> Handle(RemoveChargeFromOrderCommand request, CancellationToken cancellationToken)
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

            var Charge = order.Charges.FirstOrDefault(x => x.Id == message.ChargeId);

            if (Charge is null)
            {
                throw new Exception();
            }

            order.Charges.Remove(Charge);

            context.OrderCharges.Remove(Charge);

            order.Update();

            await context.SaveChangesAsync();

            return Unit.Value;
        }
    }
}