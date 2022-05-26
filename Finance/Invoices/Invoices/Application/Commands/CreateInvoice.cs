
using Invoices.Application;
using Invoices.Domain;
using Invoices.Domain.Enums;

using MediatR;

namespace Invoices.Application.Commands;

public record CreateInvoice(DateTime? Date, InvoiceStatus? Status, string? Note) : IRequest<InvoiceDto>
{
    public class Handler : IRequestHandler<CreateInvoice, InvoiceDto>
    {
        private readonly IInvoicesContext _context;

        public Handler(IInvoicesContext context)
        {
            _context = context;
        }

        public async Task<InvoiceDto> Handle(CreateInvoice request, CancellationToken cancellationToken)
        {
            var invoice = new Invoices.Domain.Entities.Invoice(request.Date, note: request.Note);

            _context.Invoices.Add(invoice);

            await _context.SaveChangesAsync(cancellationToken);

            return invoice.ToDto();
        }
    }
}
