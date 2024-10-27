using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Sales.Persistence;

namespace YourBrand.Sales.Features.SubscriptionManagement;

public record UpdateSubscriptionCommand(Guid SubscriptionId) : IRequest
{
    public class UpdateSubscriptionCommandHandler(SalesContext salesContext) : IRequestHandler<UpdateSubscriptionCommand>
    {
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