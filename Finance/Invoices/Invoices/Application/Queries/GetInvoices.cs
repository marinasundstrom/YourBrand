
using Invoices.Application;
using Invoices.Domain;
using Invoices.Domain.Enums;

using MassTransit;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Invoices.Application.Queries;

public record GetInvoices(int Page = 1, int PageSize = 10, InvoiceType[]? Types = null, InvoiceStatus[]? Status = null, string? Reference = null) : IRequest<ItemsResult<InvoiceDto>>
{
    public class Handler : IRequestHandler<GetInvoices, ItemsResult<InvoiceDto>>
    {
        private readonly IInvoicesContext _context;

        public Handler(IInvoicesContext context)
        {
            _context = context;
        }

        public async Task<ItemsResult<InvoiceDto>> Handle(GetInvoices request, CancellationToken cancellationToken)
        {
            if(request.PageSize < 0) 
            {
                throw new Exception("Page Size cannot be negative.");
            }

            if(request.PageSize > 100) 
            {
                throw new Exception("Page Size must not be greater than 100.");
            }

            var query = _context.Invoices
                .AsSplitQuery()
                .AsNoTracking()
                .OrderByDescending(x => x.Id)
                .AsQueryable();

            if(request.Reference is not null) 
            {
                query = query.Where(i => i.Reference!.ToLower().Contains(request.Reference.ToLower()));
            }

            if(request.Types?.Any() ?? false) 
            {
                var types = request.Types.Select(x => (int)x);
                query = query.Where(i => types.Any(s => s == (int)i.Type));
            }

            if(request.Status?.Any() ?? false) 
            {
                var statuses = request.Status.Select(x => (int)x);
                query = query.Where(i => statuses.Any(s => s == (int)i.Status));
            }

            int totalItems = await query.CountAsync(cancellationToken);

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