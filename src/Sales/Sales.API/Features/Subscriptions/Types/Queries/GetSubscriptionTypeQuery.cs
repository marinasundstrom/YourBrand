using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Sales.Features.SubscriptionManagement.Subscriptions;

namespace YourBrand.Sales.Features.SubscriptionManagement.Subscriptions.Types.Queries;

public record GetSubscriptionTypeQuery(string OrganizationId, int Id) : IRequest<SubscriptionTypeDto?>
{
    sealed class GetSubscriptionTypeQueryHandler(
        ISalesContext context,
        IUserContext userContext) : IRequestHandler<GetSubscriptionTypeQuery, SubscriptionTypeDto?>
    {
        private readonly ISalesContext _context = context;
        private readonly IUserContext userContext = userContext;

        public async Task<SubscriptionTypeDto?> Handle(GetSubscriptionTypeQuery request, CancellationToken cancellationToken)
        {
            var subscriptionType = await _context
               .SubscriptionTypes
               .Where(x => x.OrganizationId == request.OrganizationId)
               .AsNoTracking()
               .FirstAsync(c => c.Id == request.Id);

            if (subscriptionType is null)
            {
                return null;
            }

            return subscriptionType.ToDto();
        }
    }
}