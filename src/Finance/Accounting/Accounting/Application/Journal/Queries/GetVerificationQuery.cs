using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Accounting.Application.Common.Interfaces;

namespace YourBrand.Accounting.Application.Journal.Queries;

public record GetVerificationQuery(string OrganizationId, int SupportingDocumentId) : IRequest<VerificationDto>
{
    public class GetVerificationQueryHandler(IAccountingContext context) : IRequestHandler<GetVerificationQuery, VerificationDto>
    {
        public async Task<VerificationDto> Handle(GetVerificationQuery request, CancellationToken cancellationToken)
        {
            var v = await context.Verifications
                .InOrganization(request.OrganizationId)
                .Include(x => x.Entries)
                .ThenInclude(x => x.Account)
                .Include(x => x.SupportingDocuments)
                .OrderBy(x => x.Date)
                .AsNoTracking()
                .AsSplitQuery()
                .FirstOrDefaultAsync(x => x.Id == request.SupportingDocumentId, cancellationToken);

            if (v is null) throw new Exception();

            return v.ToDto();
        }
    }
}