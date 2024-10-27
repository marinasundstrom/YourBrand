using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Sales.Features.SubscriptionManagement.Subscriptions;

namespace YourBrand.Sales.Features.SubscriptionManagement.Subscriptions.Statuses.Queries;

public record GetSubscriptionStatusQuery(string OrganizationId, int Id) : IRequest<SubscriptionStatusDto?>
{
    sealed class GetSubscriptionStatusQueryHandler(
        ISalesContext context,
        IUserContext userContext) : IRequestHandler<GetSubscriptionStatusQuery, SubscriptionStatusDto?>
    {
        private readonly ISalesContext _context = context;
        private readonly IUserContext userContext = userContext;

        public async Task<SubscriptionStatusDto?> Handle(GetSubscriptionStatusQuery request, CancellationToken cancellationToken)
        {
            var subscriptionStatus = await _context
               .SubscriptionStatuses
               .Where(x => x.OrganizationId == request.OrganizationId)
               .AsNoTracking()
               .FirstAsync(c => c.Id == request.Id);

            if (subscriptionStatus is null)
            {
                return null;
            }

            return subscriptionStatus.ToDto();
        }
    }
}