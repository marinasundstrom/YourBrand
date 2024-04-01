using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Accounting.Application.Common.Interfaces;

namespace YourBrand.Accounting.Application.Journal.Queries;

public record GetJournalEntryQuery(int VerificationId) : IRequest<JournalEntryDto>
{
    public class GetJournalEntryQueryHandler : IRequestHandler<GetJournalEntryQuery, JournalEntryDto>
    {
        private readonly IAccountingContext context;

        public GetJournalEntryQueryHandler(IAccountingContext context)
        {
            this.context = context;
        }

        public async Task<JournalEntryDto> Handle(GetJournalEntryQuery request, CancellationToken cancellationToken)
        {
            var v = await context.JournalEntries
                .Include(x => x.Entries)
                .Include(x => x.Verifications)
                .OrderBy(x => x.Date)
                .AsNoTracking()
                .AsSplitQuery()
                .FirstOrDefaultAsync(x => x.Id == request.VerificationId, cancellationToken);

            if (v is null) throw new Exception();

            return v.ToDto();
        }
    }
}