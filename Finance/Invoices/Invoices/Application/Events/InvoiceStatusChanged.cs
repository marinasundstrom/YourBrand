using YourBrand.Invoices.Application.Common.Models;
using YourBrand.Invoices.Contracts;
using YourBrand.Invoices.Domain;
using YourBrand.Invoices.Domain.Enums;
using YourBrand.Invoices.Domain.Events;

using MassTransit;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace YourBrand.Invoices.Application.Events;

public class InvoiceStatusChangedHandler : INotificationHandler<DomainEventNotification<InvoiceStatusChanged>>
{
    private readonly IInvoicesContext _context;
    private readonly IPublishEndpoint _publishEndpoint;

    public InvoiceStatusChangedHandler(IInvoicesContext context, IPublishEndpoint publishEndpoint)
    {
        _context = context;
        _publishEndpoint = publishEndpoint;
    }

    public async Task Handle(DomainEventNotification<InvoiceStatusChanged> notification, CancellationToken cancellationToken)
    {
        var invoice = await _context.Invoices
            .FirstOrDefaultAsync(i => i.Id == notification.DomainEvent.InvoiceId);

        if(invoice is not null) 
        {
            if (invoice.Status == InvoiceStatus.Sent)
            {
                await _publishEndpoint.Publish(new InvoicesBatch(new[]
                {
                    new Contracts.Invoice(invoice.Id)
                }));
            }
        }
    }
}