
using Invoices.Contracts;
using Invoices.Domain;
using Invoices.Domain.Enums;

using MassTransit;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Invoices.Application.Commands;

public record SetInvoiceStatus(int InvoiceId, InvoiceStatus Status) : IRequest
{
    public class Handler : IRequestHandler<SetInvoiceStatus>
    {
        private readonly IInvoicesContext _context;

        public Handler(IInvoicesContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(SetInvoiceStatus request, CancellationToken cancellationToken)
        {
            var invoice = await _context.Invoices.FirstAsync(x => x.Id == request.InvoiceId, cancellationToken);

            invoice.SetStatus(request.Status);

            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}