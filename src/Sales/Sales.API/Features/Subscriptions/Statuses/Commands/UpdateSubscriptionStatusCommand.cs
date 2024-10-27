using MediatR;

using Microsoft.EntityFrameworkCore;

namespace YourBrand.Sales.Features.SubscriptionManagement.Subscriptions.Statuses.Commands;

public record UpdateSubscriptionStatusCommand(string OrganizationId, int Id, string Name, string Handle, string? Description) : IRequest
{
    public class UpdateSubscriptionStatusCommandHandler(ISalesContext context) : IRequestHandler<UpdateSubscriptionStatusCommand>
    {
        private readonly ISalesContext context = context;

        public async Task Handle(UpdateSubscriptionStatusCommand request, CancellationToken cancellationToken)
        {
            var subscriptionStatus = await context.SubscriptionStatuses
                    .Where(x => x.OrganizationId == request.OrganizationId)
                    .FirstOrDefaultAsync(i => i.Id == request.Id, cancellationToken);

            if (subscriptionStatus is null) throw new Exception();

            subscriptionStatus.Name = request.Name;
            subscriptionStatus.Handle = request.Handle;
            subscriptionStatus.Description = request.Description;

            await context.SaveChangesAsync(cancellationToken);
        }
    }
}