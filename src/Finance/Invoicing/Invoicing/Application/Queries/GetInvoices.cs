using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Invoicing.Domain;
using YourBrand.Invoicing.Domain.Enums;

namespace YourBrand.Invoicing.Application.Queries;

public record GetInvoices(string OrganizationId, string? CustomerId = null, int Page = 1, int PageSize = 10, InvoiceType[]? Types = null, int[]? Status = null, string? Reference = null, string? SortBy = null, SortDirection? SortDirection = null) : IRequest<ItemsResult<InvoiceDto>>
{
    public class Handler(IInvoicingContext context) : IRequestHandler<GetInvoices, ItemsResult<InvoiceDto>>
    {
        public async Task<ItemsResult<InvoiceDto>> Handle(GetInvoices request, CancellationToken cancellationToken)
        {
            if (request.PageSize < 0)
            {
                throw new Exception("Page Size cannot be negative.");
            }

            if (request.PageSize > 100)
            {
                throw new Exception("Page Size must not be greater than 100.");
            }

            var query = context.Invoices
                .Include(i => i.Status)
                .AsSplitQuery()
                .AsNoTracking()
                .OrderByDescending(x => x.Id)
                .InOrganization(request.OrganizationId)
                .AsQueryable();

            if (request.CustomerId is not null)
            {
                //int customerId = int.Parse(request.CustomerId);
                query = query.Where(i => i.Customer.Id == request.CustomerId);
            }

            if (request.Reference is not null)
            {
                query = query.Where(i => i.Reference!.ToLower().Contains(request.Reference.ToLower()));
            }

            if (request.Types?.Any() ?? false)
            {
                var types = request.Types.Select(x => (int)x);
                query = query.Where(i => types.Any(s => s == (int)i.Type));
            }

            if (request.Status?.Any() ?? false)
            {
                var statuses = request.Status.Select(x => x);
                query = query.Where(i => statuses.Any(s => s == i.Status.Id));
            }
            else
            {
                query = query.Where(x => x.StatusId != 1);
            }

            int totalItems = await query.CountAsync(cancellationToken);


            if (request.SortBy is not null)
            {
                query = query.OrderBy(request.SortBy, request.SortDirection);
            }
            else
            {
                if (!request.Status?.Any() ?? true)
                {
                    // For drafts
                    query = query.OrderByDescending(x => x.Created);
                }
                else
                {
                    query = query.OrderByDescending(x => x.IssueDate);
                }
            }

            query = query
                .Include(i => i.Items)
                .Skip(request.Page * request.PageSize)
                .Take(request.PageSize);

            var items = await query.ToArrayAsync(cancellationToken);

            return new ItemsResult<InvoiceDto>(
                items.Select(invoice => invoice.ToDto()),
                totalItems);
        }
    }
}