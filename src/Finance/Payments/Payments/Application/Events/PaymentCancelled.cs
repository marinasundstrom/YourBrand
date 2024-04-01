using Microsoft.EntityFrameworkCore;

using YourBrand.Payments.Application.Common.Interfaces;
using YourBrand.Payments.Domain;
using YourBrand.Payments.Domain.Events;
using YourBrand.Payments.Hubs;

namespace YourBrand.Payments.Application.Events;

public class PaymentCancelledHandler : IDomainEventHandler<PaymentCancelled>
{
    private readonly IPaymentsContext _context;
    private readonly IPaymentsHubClient _paymentsHubClient;

    public PaymentCancelledHandler(IPaymentsContext context, IPaymentsHubClient paymentsHubClient)
    {
        _context = context;
        _paymentsHubClient = paymentsHubClient;
    }

    public async Task Handle(PaymentCancelled notification, CancellationToken cancellationToken)
    {
        var payment = await _context
            .Payments
            .FirstOrDefaultAsync(i => i.Id == notification.PaymentId);

        if (payment is not null)
        {

        }
    }
}