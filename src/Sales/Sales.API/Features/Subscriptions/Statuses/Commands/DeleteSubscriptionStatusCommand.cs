using MediatR;

using Microsoft.EntityFrameworkCore;

namespace YourBrand.Sales.Features.SubscriptionManagement.Subscriptions.Statuses.Commands;

public record DeleteSubscriptionStatusCommand(string OrganizationId, int Id) : IRequest
{
    public class DeleteSubscriptionStatusCommandHandler(ISalesContext context) : IRequestHandler<DeleteSubscriptionStatusCommand>
    {
        private readonly ISalesContext context = context;

        public async Task Handle(DeleteSubscriptionStatusCommand request, CancellationToken cancellationToken)
        {
            var subscriptionStatus = await context.SubscriptionStatuses
                .Where(x => x.OrganizationId == request.OrganizationId)
                .FirstOrDefaultAsync(i => i.Id == request.Id, cancellationToken);

            if (subscriptionStatus is null) throw new Exception();

            context.SubscriptionStatuses.Remove(subscriptionStatus);

            await context.SaveChangesAsync(cancellationToken);
        }
    }
}