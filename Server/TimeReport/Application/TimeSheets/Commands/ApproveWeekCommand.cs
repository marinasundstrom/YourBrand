
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourCompany.TimeReport.Application.Common.Interfaces;
using YourCompany.TimeReport.Domain.Entities;
using YourCompany.TimeReport.Domain.Exceptions;

namespace YourCompany.TimeReport.Application.TimeSheets.Commands;

public class ApproveWeekCommand : IRequest
{
    public ApproveWeekCommand(string timeSheetId)
    {
        TimeSheetId = timeSheetId;
    }

    public string TimeSheetId { get; }

    public class ApproveWeekCommandHandler : IRequestHandler<ApproveWeekCommand>
    {
        private readonly ITimeReportContext _context;

        public ApproveWeekCommandHandler(ITimeReportContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(ApproveWeekCommand request, CancellationToken cancellationToken)
        {
            var timeSheet = await _context.TimeSheets
                .Include(x => x.Entries)
                .ThenInclude(x => x.Project)
                .Include(x => x.Entries)
                .ThenInclude(x => x.Activity)
                .Include(x => x.Entries)
                .ThenInclude(x => x.Activity)
                .ThenInclude(x => x.Project)
                .AsSplitQuery()
                .FirstAsync(x => x.Id == request.TimeSheetId, cancellationToken);

            if (timeSheet is null)
            {
                throw new TimeSheetNotFoundException(request.TimeSheetId);
            }

            timeSheet.Status = TimeSheetStatus.Approved;

            foreach (var entry in timeSheet.Entries)
            {
                entry.Status = EntryStatus.Locked;
            }

            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
