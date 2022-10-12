using MediatR;

using Microsoft.EntityFrameworkCore;

using OrderPriceCalculator;

using YourBrand.Orders.Contracts;
using YourBrand.Orders.Infrastructure.Persistence;

namespace YourBrand.Orders.Application.Orders.Commands;

public class ChargeDetails
{

    public decimal? Amount { get; set; }

    public double? Percent { get; set; }

    public decimal? Total { get; set; }

    public string? Description { get; set; }

    public Guid? ChargeId { get; set; }
}

public record RemoveChargeFromOrderItemCommand(int OrderNo, Guid OrderItemId, Guid ChargeId) : IRequest
{
    public class RemoveChargeFromOrderItemCommandHandler : IRequestHandler<RemoveChargeFromOrderItemCommand>
    {
        private readonly ILogger<RemoveChargeFromOrderItemCommandHandler> _logger;
        private readonly OrdersContext context;

        public RemoveChargeFromOrderItemCommandHandler(
            ILogger<RemoveChargeFromOrderItemCommandHandler> logger,
            OrdersContext context)
        {
            _logger = logger;
            this.context = context;
        }

        public async Task<Unit> Handle(RemoveChargeFromOrderItemCommand request, CancellationToken cancellationToken)
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

            var Charge = orderItem.Charges.FirstOrDefault(x => x.Id == message.ChargeId);

            if (Charge is null)
            {
                throw new Exception();
            }

            orderItem.Charges.Remove(Charge);

            context.OrderCharges.Remove(Charge);

            order.Update();

            await context.SaveChangesAsync();

            return Unit.Value;
        }
    }
}