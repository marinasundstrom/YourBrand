
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourCompany.TimeReport.Application.Common.Interfaces;
using YourCompany.TimeReport.Domain.Entities;
using YourCompany.TimeReport.Domain.Exceptions;

namespace YourCompany.TimeReport.Application.TimeSheets.Commands;

public class OpenWeekCommand : IRequest
{
    public OpenWeekCommand(string timeSheetId)
    {
        TimeSheetId = timeSheetId;
    }

    public string TimeSheetId { get; }

    public class OpenWeekCommandHandler : IRequestHandler<OpenWeekCommand>
    {
        private readonly ITimeReportContext _context;

        public OpenWeekCommandHandler(ITimeReportContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(OpenWeekCommand request, CancellationToken cancellationToken)
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

            timeSheet.Status = TimeSheetStatus.Open;

            foreach (var entry in timeSheet.Entries)
            {
                entry.Status = EntryStatus.Unlocked;
            }

            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
