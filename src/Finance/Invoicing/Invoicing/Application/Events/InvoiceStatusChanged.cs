using MassTransit;

using Microsoft.EntityFrameworkCore;

using YourBrand.Invoicing.Application.Common.Interfaces;
using YourBrand.Invoicing.Contracts;
using YourBrand.Invoicing.Domain;
using YourBrand.Invoicing.Domain.Enums;
using YourBrand.Invoicing.Domain.Events;
using YourBrand.Payments.Client;

namespace YourBrand.Invoicing.Application.Events;

public class InvoiceStatusChangedHandler(IInvoicingContext context, IPaymentsClient paymentsClient, IPublishEndpoint publishEndpoint) : IDomainEventHandler<InvoiceStatusChanged>
{
    public async Task Handle(InvoiceStatusChanged notification, CancellationToken cancellationToken)
    {
        if (notification.Status == (int)InvoiceStatus.Paid)
        {
            await publishEndpoint.Publish(new InvoicePaid(notification.InvoiceId));
            return;
        }

        var invoice = await context.Invoices
            .Include(i => i.Items)
            .FirstOrDefaultAsync(i => i.Id == notification.InvoiceId);

        if (invoice is not null)
        {
            if (invoice.Status.Id == (int)Domain.Enums.InvoiceStatus.Sent)
            {
                await publishEndpoint.Publish(new InvoicesBatch(new[]
                {
                    new Contracts.Invoice(invoice.Id)
                }));

                var dueDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.Now.AddDays(30), TimeZoneInfo.Local);

                invoice.Update();

                await paymentsClient.CreatePaymentAsync(new CreatePayment()
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