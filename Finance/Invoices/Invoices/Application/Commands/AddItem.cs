
using Invoices.Application;
using Invoices.Domain;
using Invoices.Domain.Enums;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Invoices.Application.Commands;

public record AddItem(int InvoiceId, ProductType ProductType, string Description, decimal UnitPrice, string Unit, double VatRate, double Quantity) : IRequest<InvoiceItemDto>
{
    public class Handler : IRequestHandler<AddItem, InvoiceItemDto>
    {
        private readonly IInvoicesContext _context;

        public Handler(IInvoicesContext context)
        {
            _context = context;
        }

        public async Task<InvoiceItemDto> Handle(AddItem request, CancellationToken cancellationToken)
        {
            var invoice = await _context.Invoices
                .Include(i => i.Items)
                .FirstOrDefaultAsync(x => x.Id == request.InvoiceId, cancellationToken);

            if(invoice is null) 
            {
                throw new Exception("Not found");
            }

            if(invoice.Status != InvoiceStatus.Draft) 
            {
                throw new Exception();
            }

            var item = invoice.AddItem(request.ProductType, request.Description, request.UnitPrice, request.Unit, request.VatRate, request.Quantity);

            await _context.SaveChangesAsync(cancellationToken);

            return item.ToDto();
        }
    }
}
