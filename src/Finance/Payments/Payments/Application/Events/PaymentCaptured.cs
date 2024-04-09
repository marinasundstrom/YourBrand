using MassTransit;

using Microsoft.EntityFrameworkCore;

using YourBrand.Payments.Application.Common.Interfaces;
using YourBrand.Payments.Domain;
using YourBrand.Payments.Domain.Events;
using YourBrand.Payments.Hubs;

namespace YourBrand.Payments.Application.Events;

public class PaymentCapturedHandler(IPaymentsContext context, IPaymentsHubClient paymentsHubClient, IPublishEndpoint publishEndpoint) : IDomainEventHandler<PaymentCaptured>
{
    public async Task Handle(PaymentCaptured notification, CancellationToken cancellationToken)
    {
        var payment = await context
            .Payments
            .Include(c => c.Captures)
            .FirstOrDefaultAsync(i => i.Id == notification.PaymentId);

        if (payment is not null)
        {
            var capture = payment.Captures.FirstOrDefault(c => c.Id == notification.CaptureId);

            if (capture is not null)
            {
                await publishEndpoint.Publish(new Contracts.PaymentCaptured(capture.PaymentId, capture.Id, capture.Date, payment.Currency, capture.Amount));
            }
        }
    }
}