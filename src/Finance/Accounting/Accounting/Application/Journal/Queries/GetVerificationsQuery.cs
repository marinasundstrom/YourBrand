using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Accounting.Application.Common.Interfaces;

namespace YourBrand.Accounting.Application.Journal.Queries;

public record GetVerificationsQuery(string OrganizationId, int Page = 0, int PageSize = 10, int? InvoiceNo = null) : IRequest<VerificationResult>
{
    public class GetVerificationsQueryHandler(IAccountingContext context) : IRequestHandler<GetVerificationsQuery, VerificationResult>
    {
        public async Task<VerificationResult> Handle(GetVerificationsQuery request, CancellationToken cancellationToken)
        {
            if (request.PageSize < 0)
            {
                throw new Exception("Page Size cannot be negative.");
            }

            if (request.PageSize > 100)
            {
                throw new Exception("Page Size must not be greater than 100.");
            }

            var query = context.Verifications
                .InOrganization(request.OrganizationId)
                .Include(x => x.Entries)
                .ThenInclude(x => x.Account)
                .Include(x => x.SupportingDocuments)
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

            var vms = new List<VerificationDto>();

            vms.AddRange(r.Select(v => v.ToDto()));

            return new VerificationResult(vms, totalItems);
        }
    }
}