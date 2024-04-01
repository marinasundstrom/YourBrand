using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Sales.Persistence;

namespace YourBrand.Sales.Features.Subscriptions;

public record UpdateSubscriptionCommand(Guid SubscriptionId) : IRequest
{
    public class UpdateSubscriptionCommandHandler : IRequestHandler<UpdateSubscriptionCommand>
    {
        private readonly SalesContext salesContext;

        public UpdateSubscriptionCommandHandler(SalesContext salesContext)
        {
            this.salesContext = salesContext;
        }

        public async Task Handle(UpdateSubscriptionCommand request, CancellationToken cancellationToken)
        {
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