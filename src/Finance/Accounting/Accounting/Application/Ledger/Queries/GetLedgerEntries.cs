using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Accounting.Application.Common.Interfaces;

namespace YourBrand.Accounting.Application.Ledger.Queries;

public record GetLedgerEntriesQuery(int? AccountNo = null, int? VerificationId = null, int Page = 0, int PageSize = 10, ResultDirection Direction = ResultDirection.Asc) : IRequest<LedgerEntriesResult>
{
    public class GetLedgerEntriesQueryHandler(IAccountingContext context) : IRequestHandler<GetLedgerEntriesQuery, LedgerEntriesResult>
    {
        public async Task<LedgerEntriesResult> Handle(GetLedgerEntriesQuery request, CancellationToken cancellationToken)
        {
            if (request.PageSize < 0)
            {
                throw new Exception("Page Size cannot be negative.");
            }

            if (request.PageSize > 100)
            {
                throw new Exception("Page Size must not be greater than 100.");
            }

            var query = context.LedgerEntries
                   .Include(e => e.JournalEntry)
                   .Include(e => e.Account)
                   .AsNoTracking()
                   .AsQueryable();

            if (request.Direction == ResultDirection.Asc)
            {
                query = query.OrderBy(e => e.Date);
            }
            else
            {
                query = query.OrderByDescending(e => e.Date);
            }

            if (request.AccountNo is not null)
            {
                query = query.Where(e => e.AccountNo == request.AccountNo);
            }

            if (request.VerificationId is not null)
            {
                query = query.Where(e => e.JournalEntryId == request.VerificationId);
            }

            var totalItems = await query.CountAsync(cancellationToken);

            var entries = await query
                .Skip(request.PageSize * request.Page)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            var entries2 = entries.Select(e => e.ToDto());

            return new LedgerEntriesResult(entries2, totalItems);
        }
    }
}