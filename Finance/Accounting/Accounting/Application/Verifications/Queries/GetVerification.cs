using Accounting.Application.Common.Interfaces;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Accounting.Application.Verifications.Queries;

public record GetVerificationQuery(int VerificationId) : IRequest<VerificationDto>
{
    public class GetVerificationQueryHandler : IRequestHandler<GetVerificationQuery, VerificationDto>
    {
        private readonly IAccountingContext context;

        public GetVerificationQueryHandler(IAccountingContext context)
        {
            this.context = context;
        }

        public async Task<VerificationDto> Handle(GetVerificationQuery request, CancellationToken cancellationToken)
        {
            var v = await context.Verifications
                .Include(x => x.Entries)
                .Include(x => x.Attachments)
                .OrderBy(x => x.Date)
                .AsNoTracking()
                .AsSplitQuery()
                .FirstOrDefaultAsync(x => x.Id == request.VerificationId, cancellationToken);

            if (v is null) throw new Exception();

            return v.ToDto();
        }
    }
}