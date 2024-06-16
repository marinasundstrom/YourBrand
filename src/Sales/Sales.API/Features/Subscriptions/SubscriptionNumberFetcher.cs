using Microsoft.EntityFrameworkCore;

using YourBrand.Sales.Persistence;

namespace YourBrand.Sales.Features.Subscriptions;

public sealed class SubscriptionNumberFetcher(SalesContext salesContext)
{
    public async Task<int> GetNextNumberAsync(string organizationId, CancellationToken cancellationToken)
    {
        int subscriptionNo;

        try
        {
            subscriptionNo = (await salesContext.Subscriptions
                .InOrganization(organizationId)
                .MaxAsync(x => x.SubscriptionNo, cancellationToken: cancellationToken)) + 1;
        }
        catch (InvalidOperationException e)
        {
            subscriptionNo = 1; // Subscription start number
        }

        return subscriptionNo;
    }
}