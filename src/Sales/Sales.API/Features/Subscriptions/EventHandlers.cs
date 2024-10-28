using Microsoft.EntityFrameworkCore;

using YourBrand.Domain;
using YourBrand.Sales.Domain.Entities;
using YourBrand.Sales.Persistence;

namespace YourBrand.Sales.Features.SubscriptionManagement;

public sealed class SubscriptionActivatedEventHandler(SalesContext salesContext, ILogger<SubscriptionActivatedEventHandler> logger) : IDomainEventHandler<SubscriptionActivated>
{
    public async Task Handle(SubscriptionActivated notification, CancellationToken cancellationToken)
    {
        var subscription = await salesContext.Subscriptions
            .InOrganization(notification.OrganizationId)
            .Include(x => x.Plan)
            .FirstOrDefaultAsync(x => x.Id == notification.SubscriptionId, cancellationToken);

        if (subscription is { Plan: { PlanType: SubscriptionPlanType.RecurringOrder } })
        {

        }

        logger.LogInformation("Subscription activated");
    }
}

public sealed class SubscriptionCanceledEventHandler(ILogger<SubscriptionCanceledEventHandler> logger) : IDomainEventHandler<SubscriptionCanceled>
{
    public Task Handle(SubscriptionCanceled notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("Subscription canceled");
        return Task.CompletedTask;
    }
}