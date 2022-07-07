using YourBrand.Invoicing.Application.Common.Models;
using YourBrand.Invoicing.Contracts;
using YourBrand.Invoicing.Domain;
using YourBrand.Invoicing.Domain.Enums;
using YourBrand.Invoicing.Domain.Events;

using MassTransit;

using MediatR;

using Microsoft.EntityFrameworkCore;
using YourBrand.Payments.Client;

namespace YourBrand.Invoicing.Application.Events;

public class InvoiceStatusChangedHandler : INotificationHandler<DomainEventNotification<InvoiceStatusChanged>>
{
    private readonly IInvoicingContext _context;
    private readonly IPaymentsClient _paymentsClient;
    private readonly IPublishEndpoint _publishEndpoint;

    public InvoiceStatusChangedHandler(IInvoicingContext context, IPaymentsClient paymentsClient, IPublishEndpoint publishEndpoint)
    {
        _context = context;
        _paymentsClient = paymentsClient;
        _publishEndpoint = publishEndpoint;
    }

    public async Task Handle(DomainEventNotification<InvoiceStatusChanged> notification, CancellationToken cancellationToken)
    {
        if (notification.DomainEvent.Status == InvoiceStatus.Paid)
        {
            await _publishEndpoint.Publish(new InvoicePaid(notification.DomainEvent.InvoiceId));
            return;
        }

        var invoice = await _context.Invoices
            .Include(i => i.Items)
            .FirstOrDefaultAsync(i => i.Id == notification.DomainEvent.InvoiceId);

        if (invoice is not null)
        {
            if (invoice.Status == InvoiceStatus.Sent)
            {
                await _publishEndpoint.Publish(new InvoicesBatch(new[]
                {
                    new Contracts.Invoice(invoice.Id)
                }));

                var dueDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.Now.AddDays(30), TimeZoneInfo.Local);

                invoice.UpdateTotals();

                await _paymentsClient.CreatePaymentAsync(new CreatePayment()
                {
                    InvoiceId = invoice.Id,
                    Currency = "SEK",
                    Amount = invoice.Total,
                    PaymentMethod = PaymentMethod.PlusGiro,
                    DueDate = dueDate,
                    Reference = Guid.NewGuid().ToUrlFriendlyString(),
                    Message = $"Betala faktura #{invoice.Id}",
                });
            }
        }
    }
}