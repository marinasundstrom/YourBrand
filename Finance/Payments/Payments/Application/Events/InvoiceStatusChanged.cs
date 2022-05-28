using YourBrand.Payments.Application.Common.Models;
using YourBrand.Payments.Domain;
using YourBrand.Payments.Domain.Events;

using MediatR;

using Microsoft.EntityFrameworkCore;
using YourBrand.Payments.Hubs;

namespace YourBrand.Payments.Application.Events;

public class PaymentStatusChangedHandler : INotificationHandler<DomainEventNotification<PaymentStatusChanged>>
{
    private readonly IPaymentsContext _context;
    private readonly IPaymentsHubClient _paymentsHubClient;

    public PaymentStatusChangedHandler(IPaymentsContext context, IPaymentsHubClient paymentsHubClient)
    {
        _context = context;
        _paymentsHubClient = paymentsHubClient;
    }

    public async Task Handle(DomainEventNotification<PaymentStatusChanged> notification, CancellationToken cancellationToken)
    {
        var payment = await _context
            .Payments
            .FirstOrDefaultAsync(i => i.Id == notification.DomainEvent.PaymentId);

        if(payment is not null) 
        {
            await _paymentsHubClient.PaymentStatusUpdated(payment.Id, payment.Status);
        }
    }
}