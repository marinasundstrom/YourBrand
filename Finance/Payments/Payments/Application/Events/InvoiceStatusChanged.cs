using YourBrand.Payments.Application.Common.Models;
using YourBrand.Payments.Domain;
using YourBrand.Payments.Domain.Events;

using MediatR;

using Microsoft.EntityFrameworkCore;
using YourBrand.Payments.Hubs;
using MassTransit;

namespace YourBrand.Payments.Application.Events;

public class PaymentStatusChangedHandler : INotificationHandler<DomainEventNotification<PaymentStatusChanged>>
{
    private readonly IPaymentsContext _context;
    private readonly IPaymentsHubClient _paymentsHubClient;
    private readonly IPublishEndpoint _publishEndpoint;

    public PaymentStatusChangedHandler(IPaymentsContext context, IPaymentsHubClient paymentsHubClient, IPublishEndpoint publishEndpoint)
    {
        _context = context;
        _paymentsHubClient = paymentsHubClient;
        _publishEndpoint = publishEndpoint;
    }

    public async Task Handle(DomainEventNotification<PaymentStatusChanged> notification, CancellationToken cancellationToken)
    {
        var payment = await _context
            .Payments
            .FirstOrDefaultAsync(i => i.Id == notification.DomainEvent.PaymentId);

        if(payment is not null) 
        {
            await _publishEndpoint.Publish(new Contracts.PaymentStatusChanged(payment.Id, (Contracts.PaymentStatus)payment.Status));
            await _paymentsHubClient.PaymentStatusUpdated(payment.Id, payment.Status);
        }
    }
}
