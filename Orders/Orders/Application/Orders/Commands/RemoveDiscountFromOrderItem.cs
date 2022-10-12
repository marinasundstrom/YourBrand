using MediatR;

using Microsoft.EntityFrameworkCore;

using OrderPriceCalculator;

using YourBrand.Orders.Contracts;
using YourBrand.Orders.Infrastructure.Persistence;

namespace YourBrand.Orders.Application.Orders.Commands;

public class RemoveDiscountFromOrderItemCommand : IRequest
{
    public RemoveDiscountFromOrderItemCommand(
        int orderNo,
        Guid orderItemId,
        Guid discountId
    )
    {
        OrderNo = orderNo;
        OrderItemId = orderItemId;
        DiscountId = discountId;
    }

    public int OrderNo { get; }

    public Guid OrderItemId { get; }

    public Guid DiscountId { get; }

    public class RemoveDiscountFromOrderItemCommandHandler : IRequestHandler<RemoveDiscountFromOrderItemCommand>
    {
        private readonly ILogger<RemoveDiscountFromOrderItemCommandHandler> _logger;
        private readonly OrdersContext context;
 
        public RemoveDiscountFromOrderItemCommandHandler(
            ILogger<RemoveDiscountFromOrderItemCommandHandler> logger,
            OrdersContext context)
        {
            _logger = logger;
            this.context = context;
        }

        public async Task<Unit> Handle(RemoveDiscountFromOrderItemCommand request, CancellationToken cancellationToken)
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

            var discount = orderItem.Discounts.FirstOrDefault(x => x.Id == message.DiscountId);

            if (discount is null)
            {
                throw new Exception();
            }

            orderItem.Discounts.Remove(discount);

            context.OrderDiscounts.Remove(discount);

            order.Update();

            await context.SaveChangesAsync();

            return Unit.Value;
        }
    }
}