using MassTransit;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Payments.Application.Common.Models;
using YourBrand.Payments.Domain;
using YourBrand.Payments.Domain.Events;
using YourBrand.Payments.Hubs;

namespace YourBrand.Payments.Application.Events;

public class PaymentCapturedHandler : INotificationHandler<DomainEventNotification<PaymentCaptured>>
{
    private readonly IPaymentsContext _context;
    private readonly IPaymentsHubClient _paymentsHubClient;
    private readonly IPublishEndpoint _publishEndpoint;

    public PaymentCapturedHandler(IPaymentsContext context, IPaymentsHubClient paymentsHubClient, IPublishEndpoint publishEndpoint)
    {
        _context = context;
        _paymentsHubClient = paymentsHubClient;
        _publishEndpoint = publishEndpoint;
    }

    public async Task Handle(DomainEventNotification<PaymentCaptured> notification, CancellationToken cancellationToken)
    {
        var payment = await _context
            .Payments
            .Include(c => c.Captures)
            .FirstOrDefaultAsync(i => i.Id == notification.DomainEvent.PaymentId);

        if (payment is not null) 
        {
            var capture = payment.Captures.FirstOrDefault(c => c.Id == notification.DomainEvent.CaptureId);

            if (capture is not null)
            {
                await _publishEndpoint.Publish(new Contracts.PaymentCaptured(capture.PaymentId, capture.Id, capture.Date, payment.Currency, capture.Amount));
            }
        }
    }
}