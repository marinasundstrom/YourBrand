
using YourBrand.Invoicing.Application;
using YourBrand.Invoicing.Domain;
using YourBrand.Invoicing.Domain.Enums;

using MediatR;

namespace YourBrand.Invoicing.Application.Commands;

public record CreateInvoice(DateTime? Date, InvoiceStatus? Status, string? Note) : IRequest<InvoiceDto>
{
    public class Handler : IRequestHandler<CreateInvoice, InvoiceDto>
    {
        private readonly IInvoicingContext _context;

        public Handler(IInvoicingContext context)
        {
            _context = context;
        }

        public async Task<InvoiceDto> Handle(CreateInvoice request, CancellationToken cancellationToken)
        {
            var invoice = new YourBrand.Invoicing.Domain.Entities.Invoice(request.Date, note: request.Note);

            _context.Invoices.Add(invoice);

            await _context.SaveChangesAsync(cancellationToken);

            return invoice.ToDto();
        }
    }
}
