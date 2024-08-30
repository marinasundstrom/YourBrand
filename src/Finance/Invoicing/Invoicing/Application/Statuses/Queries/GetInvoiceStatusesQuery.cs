using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;
using YourBrand.Invoicing;
using YourBrand.Invoicing.Application;
using YourBrand.Invoicing.Domain;
using YourBrand.Invoicing.Domain.Entities;

namespace YourBrand.Sales.Features.InvoiceManagement.Invoices.Statuses.Queries;

public record GetInvoiceStatusesQuery(string OrganizationId, int Page = 0, int PageSize = 10, string? SearchString = null, string? SortBy = null, SortDirection? SortDirection = null) : IRequest<ItemsResult<InvoiceStatusDto>>
{
    sealed class GetInvoiceStatusesQueryHandler(
        IInvoicingContext context,
        IUserContext userContext) : IRequestHandler<GetInvoiceStatusesQuery, ItemsResult<InvoiceStatusDto>>
    {
        private readonly IInvoicingContext _context = context;
        private readonly IUserContext userContext = userContext;

        public async Task<ItemsResult<InvoiceStatusDto>> Handle(GetInvoiceStatusesQuery request, CancellationToken cancellationToken)
        {
            IQueryable<InvoiceStatus> result = _context
                    .InvoiceStatuses
                    .Where(x => x.OrganizationId == request.OrganizationId)
                    .OrderBy(o => o.Created)
                    .AsNoTracking()
                    .AsQueryable();

            if (request.SearchString is not null)
            {
                result = result.Where(o => o.Name.ToLower().Contains(request.SearchString.ToLower()));
            }

            var totalCount = await result.CountAsync(cancellationToken);

            if (request.SortBy is not null)
            {
                result = result.OrderBy(request.SortBy, request.SortDirection);
            }
            else
            {
                result = result.OrderBy(x => x.Id);
            }

            var items = await result
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToArrayAsync(cancellationToken);

            return new ItemsResult<InvoiceStatusDto>(items.Select(cp => cp.ToDto()), totalCount);
        }
    }
}