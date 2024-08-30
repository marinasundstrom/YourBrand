using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;
using YourBrand.Invoicing.Application;
using YourBrand.Invoicing.Domain;

namespace YourBrand.Sales.Features.InvoiceManagement.Invoices.Statuses.Queries;

public record GetInvoiceStatusQuery(string OrganizationId, int Id) : IRequest<InvoiceStatusDto?>
{
    sealed class GetInvoiceStatusQueryHandler(
        IInvoicingContext context,
        IUserContext userContext) : IRequestHandler<GetInvoiceStatusQuery, InvoiceStatusDto?>
    {
        private readonly IInvoicingContext _context = context;
        private readonly IUserContext userContext = userContext;

        public async Task<InvoiceStatusDto?> Handle(GetInvoiceStatusQuery request, CancellationToken cancellationToken)
        {
            var invoiceStatus = await _context
               .InvoiceStatuses
               .Where(x => x.OrganizationId == request.OrganizationId)
               .AsNoTracking()
               .FirstAsync(c => c.Id == request.Id);

            if (invoiceStatus is null)
            {
                return null;
            }

            return invoiceStatus.ToDto();
        }
    }
}