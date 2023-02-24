
using YourBrand.Invoicing.Contracts;
using YourBrand.Invoicing.Domain;
using YourBrand.Invoicing.Domain.Enums;

using MassTransit;

using MediatR;

using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace YourBrand.Invoicing.Application.Commands;

public record SetInvoiceStatus(string InvoiceId, InvoiceStatus Status) : IRequest
{
    public class Handler : IRequestHandler<SetInvoiceStatus>
    {
        private readonly IInvoicingContext _context;

        public Handler(IInvoicingContext context)
        {
            _context = context;
        }

        public async Task Handle(SetInvoiceStatus request, CancellationToken cancellationToken)
        {
            var invoice = await _context.Invoices.FirstAsync(x => x.Id == request.InvoiceId, cancellationToken);

            invoice.SetStatus(request.Status);

            await _context.SaveChangesAsync(cancellationToken);

        }
    }
}
