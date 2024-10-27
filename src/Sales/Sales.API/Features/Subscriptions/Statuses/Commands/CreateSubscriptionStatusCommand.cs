using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Sales.Features.SubscriptionManagement.Subscriptions;

namespace YourBrand.Sales.Features.SubscriptionManagement.Subscriptions.Statuses.Commands;

public record CreateSubscriptionStatusCommand(string OrganizationId, string Name, string Handle, string? Description) : IRequest<SubscriptionStatusDto>
{
    public class CreateSubscriptionStatusCommandHandler(ISalesContext context) : IRequestHandler<CreateSubscriptionStatusCommand, SubscriptionStatusDto>
    {
        private readonly ISalesContext context = context;

        public async Task<SubscriptionStatusDto> Handle(CreateSubscriptionStatusCommand request, CancellationToken cancellationToken)
        {
            var subscriptionStatus = await context.SubscriptionStatuses.FirstOrDefaultAsync(i => i.Name == request.Name, cancellationToken);

            if (subscriptionStatus is not null) throw new Exception();

            int subscriptionStatusNo = 1;

            try
            {
                subscriptionStatusNo = await context.SubscriptionStatuses
                    .Where(x => x.OrganizationId == request.OrganizationId)
                    .MaxAsync(x => x.Id, cancellationToken) + 1;
            }
            catch { }

            subscriptionStatus = new Domain.Entities.SubscriptionStatus(subscriptionStatusNo, request.Name, request.Handle, request.Description);
            subscriptionStatus.OrganizationId = request.OrganizationId;

            context.SubscriptionStatuses.Add(subscriptionStatus);

            await context.SaveChangesAsync(cancellationToken);

            return subscriptionStatus.ToDto();
        }
    }
}