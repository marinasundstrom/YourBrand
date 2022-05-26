using Accounting.Application.Common.Interfaces;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Accounting.Application.Verifications.Queries;

public record GetVerificationsQuery(int Page = 0, int PageSize = 10) : IRequest<VerificationsResult>
{
    public class GetVerificationsQueryHandler : IRequestHandler<GetVerificationsQuery, VerificationsResult>
    {
        private readonly IAccountingContext context;

        public GetVerificationsQueryHandler(IAccountingContext context)
        {
            this.context = context;
        }

        public async Task<VerificationsResult> Handle(GetVerificationsQuery request, CancellationToken cancellationToken)
        {
            if(request.PageSize < 0) 
            {
                throw new Exception("Page Size cannot be negative.");
            }

            if(request.PageSize > 100) 
            {
                throw new Exception("Page Size must not be greater than 100.");
            }
            
            var query = context.Verifications
                .Include(x => x.Entries)
                .Include(x => x.Attachments)
                .OrderBy(x => x.Date)
                .AsNoTracking()
                .AsSplitQuery()
                .AsQueryable();

            var totalItems = await query.CountAsync();

            var r = await query
               .Skip(request.PageSize * request.Page)
               .Take(request.PageSize)
               .ToListAsync(cancellationToken);

            var vms = new List<VerificationDto>();

            vms.AddRange(r.Select(v => v.ToDto()));

            return new VerificationsResult(vms, totalItems);
        }
    }
}