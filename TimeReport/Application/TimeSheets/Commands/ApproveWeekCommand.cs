
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.TimeReport.Application.Common.Interfaces;
using YourBrand.TimeReport.Domain.Entities;
using YourBrand.TimeReport.Domain.Exceptions;

namespace YourBrand.TimeReport.Application.TimeSheets.Commands;

public record ApproveWeekCommand(string TimeSheetId) : IRequest
{
    public class ApproveWeekCommandHandler : IRequestHandler<ApproveWeekCommand>
    {
        private readonly ITimeReportContext _context;

        public ApproveWeekCommandHandler(ITimeReportContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(ApproveWeekCommand request, CancellationToken cancellationToken)
        {
            var timeSheet = await _context.TimeSheets.GetTimeSheetAsync(request.TimeSheetId, cancellationToken);

            if (timeSheet is null)
            {
                throw new TimeSheetNotFoundException(request.TimeSheetId);
            }

            timeSheet.UpdateStatus(TimeSheetStatus.Approved);

            foreach (var entry in timeSheet.Entries)
            {
                entry.UpdateStatus(EntryStatus.Locked);
            }

            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
