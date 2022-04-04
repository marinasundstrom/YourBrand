
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.TimeReport.Application.Common.Interfaces;
using YourBrand.TimeReport.Domain.Entities;
using YourBrand.TimeReport.Domain.Exceptions;

namespace YourBrand.TimeReport.Application.TimeSheets.Commands;

public class CloseWeekCommand : IRequest
{
    public CloseWeekCommand(string timeSheetId)
    {
        TimeSheetId = timeSheetId;
    }

    public string TimeSheetId { get; }

    public class CloseWeekCommandHandler : IRequestHandler<CloseWeekCommand>
    {
        private readonly ITimeReportContext _context;

        public CloseWeekCommandHandler(ITimeReportContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(CloseWeekCommand request, CancellationToken cancellationToken)
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

            timeSheet.Status = TimeSheetStatus.Closed;

            foreach (var entry in timeSheet.Entries)
            {
                entry.Status = EntryStatus.Locked;
            }

            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
