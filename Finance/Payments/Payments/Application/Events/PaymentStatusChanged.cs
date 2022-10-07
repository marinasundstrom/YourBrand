using YourBrand.Payments.Application.Common.Models;
using YourBrand.Payments.Domain;
using YourBrand.Payments.Domain.Events;

using MediatR;

using Microsoft.EntityFrameworkCore;
using YourBrand.Payments.Hubs;
using MassTransit;
using YourBrand.Payments.Application.Common.Interfaces;

namespace YourBrand.Payments.Application.Events;

public class PaymentStatusChangedHandler : IDomainEventHandler<PaymentStatusChanged>
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

    public async Task Handle(PaymentStatusChanged notification, CancellationToken cancellationToken)
    {
        var payment = await _context
            .Payments
            .FirstOrDefaultAsync(i => i.Id == notification.PaymentId);

        if(payment is not null) 
        {
            await _publishEndpoint.Publish(new Contracts.PaymentStatusChanged(payment.Id, (Contracts.PaymentStatus)payment.Status));
            await _paymentsHubClient.PaymentStatusUpdated(payment.Id, payment.Status);
        }
    }
}
