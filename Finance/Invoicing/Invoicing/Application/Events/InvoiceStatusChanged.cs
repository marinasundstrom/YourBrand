using YourBrand.Invoicing.Application.Common.Models;
using YourBrand.Invoicing.Contracts;
using YourBrand.Invoicing.Domain;
using YourBrand.Invoicing.Domain.Enums;
using YourBrand.Invoicing.Domain.Events;

using MassTransit;

using Microsoft.EntityFrameworkCore;
using YourBrand.Payments.Client;
using YourBrand.Invoicing.Application.Common.Interfaces;

namespace YourBrand.Invoicing.Application.Events;

public class InvoiceStatusChangedHandler : IDomainEventHandler<InvoiceStatusChanged>
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

    public async Task Handle(InvoiceStatusChanged notification, CancellationToken cancellationToken)
    {
        if (notification.Status == InvoiceStatus.Paid)
        {
            await _publishEndpoint.Publish(new InvoicePaid(notification.InvoiceId));
            return;
        }

        var invoice = await _context.Invoices
            .Include(i => i.Items)
            .FirstOrDefaultAsync(i => i.Id == notification.InvoiceId);

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