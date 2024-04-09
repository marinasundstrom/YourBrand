using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Accounting.Application.Common.Interfaces;

namespace YourBrand.Accounting.Application.Journal.Queries;

public record GetJournalEntriesQuery(int Page = 0, int PageSize = 10, int? InvoiceNo = null) : IRequest<JournalEntryResult>
{
    public class GetJournalEntriesQueryHandler(IAccountingContext context) : IRequestHandler<GetJournalEntriesQuery, JournalEntryResult>
    {
        public async Task<JournalEntryResult> Handle(GetJournalEntriesQuery request, CancellationToken cancellationToken)
        {
            if (request.PageSize < 0)
            {
                throw new Exception("Page Size cannot be negative.");
            }

            if (request.PageSize > 100)
            {
                throw new Exception("Page Size must not be greater than 100.");
            }

            var query = context.JournalEntries
                .Include(x => x.Entries)
                .Include(x => x.Verifications)
                .OrderBy(x => x.Date)
                .AsNoTracking()
                .AsSplitQuery()
                .AsQueryable();

            if (request.InvoiceNo is not null)
            {
                query = query.Where(x => x.InvoiceNo == request.InvoiceNo);
            }

            var totalItems = await query.CountAsync();

            var r = await query
               .Skip(request.PageSize * request.Page)
               .Take(request.PageSize)
               .ToListAsync(cancellationToken);

            var vms = new List<JournalEntryDto>();

            vms.AddRange(r.Select(v => v.ToDto()));

            return new JournalEntryResult(vms, totalItems);
        }
    }
}