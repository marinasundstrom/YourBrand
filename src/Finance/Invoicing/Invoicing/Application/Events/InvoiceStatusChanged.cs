using MassTransit;

using Microsoft.EntityFrameworkCore;

using Quartz.Xml.JobSchedulingData20;

using YourBrand.Invoicing.Application.Common.Interfaces;
using YourBrand.Invoicing.Contracts;
using YourBrand.Invoicing.Domain;
using YourBrand.Invoicing.Domain.Enums;
using YourBrand.Invoicing.Domain.Events;
using YourBrand.Payments.Client;

namespace YourBrand.Invoicing.Application.Events;

public class InvoiceStatusChangedHandler(IInvoicingContext context, InvoiceNumberFetcher invoiceNumberFetcher, IPaymentsClient paymentsClient, IPublishEndpoint publishEndpoint) : IDomainEventHandler<InvoiceStatusUpdated>
{
    public async Task Handle(InvoiceStatusUpdated notification, CancellationToken cancellationToken)
    {
        if (notification.Status == (int)InvoiceStatus.Paid)
        {
            await publishEndpoint.Publish(new InvoicePaid(notification.OrganizationId, notification.InvoiceId));
        }

        var invoice = await context.Invoices
            .IgnoreQueryFilters()
            .Include(i => i.Items)
            .FirstOrDefaultAsync(i => i.Id == notification.InvoiceId);

        if (invoice is not null)
        {
            if (notification.OldStatus == 1)
            {
                await invoice.AssignInvoiceNo(invoiceNumberFetcher, cancellationToken);

                await context.SaveChangesAsync(cancellationToken);
            }

            if (invoice.Status.Id == (int)Domain.Enums.InvoiceStatus.Sent)
            {
                await publishEndpoint.Publish(new InvoicesBatch(invoice.OrganizationId, new[]
                {
                    new Contracts.Invoice(invoice.OrganizationId, invoice.Id)
                }));

                var dueDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.Now.AddDays(30), TimeZoneInfo.Local);

                invoice.Update();

                /*
                await paymentsClient.CreatePaymentAsync(invoice.OrganizationId, ()
                {
                    InvoiceId = invoice.Id,
                    Currency = "SEK",
                    Amount = invoice.Total,
                    PaymentMethod = PaymentMethod.PlusGiro,
                    DueDate = dueDate,
                    Reference = Guid.NewGuid().ToUrlFriendlyString(),
                    Message = $"Betala faktura #{invoice.Id}",
                }); */
            }
        }
    }
}