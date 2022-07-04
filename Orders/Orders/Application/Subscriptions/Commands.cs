using System.Threading;
using System.Threading.Tasks;

using MassTransit;

using Microsoft.EntityFrameworkCore;

using YourBrand.Orders.Contracts;
using YourBrand.Orders.Domain.Entities;
using YourBrand.Orders.Infrastructure.Persistence;

using static YourBrand.Orders.Application.Subscriptions.Mappings;

namespace YourBrand.Orders.Application.Subscriptions
{
    public class UpdateSubscriptionCommandHandler : IConsumer<UpdateSubscriptionCommand>
    {
        private readonly OrdersContext salesContext;

        public UpdateSubscriptionCommandHandler(OrdersContext salesContext)
        {
            this.salesContext = salesContext;
        }

        public async Task Consume(ConsumeContext<UpdateSubscriptionCommand> consumeContext)
        {
            var request = consumeContext.Message;

            var subscription = await salesContext.Subscriptions
                .FirstOrDefaultAsync(c => c.Id == request.SubscriptionId);

            if (subscription is null)
            {
                throw new System.Exception();
            }

            // TODO: Update

            await salesContext.SaveChangesAsync();
        }
    }
}