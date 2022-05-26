using Accounting.Application.Common.Interfaces;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Accounting.Application.Entries.Queries;

public record GetEntriesQuery(int? AccountNo = null, int? VerificationId = null, int Page = 0, int PageSize = 10, ResultDirection Direction = ResultDirection.Asc) : IRequest<EntriesResult>
{
    public class GetEntriesQueryHandler : IRequestHandler<GetEntriesQuery, EntriesResult>
    {
        private readonly IAccountingContext context;

        public GetEntriesQueryHandler(IAccountingContext context)
        {
            this.context = context;
        }

        public async Task<EntriesResult> Handle(GetEntriesQuery request, CancellationToken cancellationToken)
        {
            if(request.PageSize < 0) 
            {
                throw new Exception("Page Size cannot be negative.");
            }

            if(request.PageSize > 100) 
            {
                throw new Exception("Page Size must not be greater than 100.");
            }
            
            var query = context.Entries
                   .Include(e => e.Verification)
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
                query = query.Where(e => e.VerificationId == request.VerificationId);
            }

            var totalItems = await query.CountAsync(cancellationToken);

            var entries = await query
                .Skip(request.PageSize * request.Page)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            var entries2 = entries.Select(e => e.ToDto());

            return new EntriesResult(entries2, totalItems);
        }
    }
}