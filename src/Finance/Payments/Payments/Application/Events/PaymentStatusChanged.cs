using MassTransit;

using Microsoft.EntityFrameworkCore;

using YourBrand.Payments.Application.Common.Interfaces;
using YourBrand.Payments.Domain;
using YourBrand.Payments.Domain.Events;
using YourBrand.Payments.Hubs;

namespace YourBrand.Payments.Application.Events;

public class PaymentStatusChangedHandler(IPaymentsContext context, IPaymentsHubClient paymentsHubClient, IPublishEndpoint publishEndpoint) : IDomainEventHandler<PaymentStatusChanged>
{
    public async Task Handle(PaymentStatusChanged notification, CancellationToken cancellationToken)
    {
        var payment = await context
            .Payments
            .FirstOrDefaultAsync(i => i.Id == notification.PaymentId);

        if (payment is not null)
        {
            await publishEndpoint.Publish(new Contracts.PaymentStatusChanged(payment.OrganizationId, payment.Id, (Contracts.PaymentStatus)payment.Status));
            await paymentsHubClient.PaymentStatusUpdated(payment.Id, payment.Status);
        }
    }
}