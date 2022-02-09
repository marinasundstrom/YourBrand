
using MediatR;

using Microsoft.EntityFrameworkCore;

using TimeReport.Application.Common.Interfaces;
using TimeReport.Domain.Entities;
using TimeReport.Domain.Exceptions;

namespace TimeReport.Application.TimeSheets.Commands;

public class LockMonthCommand : IRequest
{
    public LockMonthCommand(string timeSheetId)
    {
        TimeSheetId = timeSheetId;
    }

    public string TimeSheetId { get; }

    public class LockMonthCommandHandler : IRequestHandler<LockMonthCommand>
    {
        private readonly ITimeReportContext _context;

        public LockMonthCommandHandler(ITimeReportContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(LockMonthCommand request, CancellationToken cancellationToken)
        {
            var timeSheet = await _context.TimeSheets
                        .Include(x => x.Entries)
                        .Include(x => x.User)
                        .AsSplitQuery()
                        .FirstAsync(x => x.Id == request.TimeSheetId);

            if (timeSheet is null)
            {
                throw new TimeSheetNotFoundException(request.TimeSheetId);
            }

            var firstWeekDay = timeSheet.From;
            var lastWeekDay = timeSheet.To;

            int month = firstWeekDay.Month;

            DateTime firstDate;
            DateTime lastDate;

            if (firstWeekDay.Month == lastWeekDay.Month)
            {
                int daysInMonth = DateTime.DaysInMonth(firstWeekDay.Month, month);

                if (lastWeekDay.Month == daysInMonth)
                {
                    firstDate = new DateTime(firstWeekDay.Year, firstWeekDay.Month, 1);
                    lastDate = lastWeekDay;
                }
                else
                {
                    /*
                    return Problem(
                              title: "Failed to lock month",
                              detail: $"Unable to lock month in this timesheet.",
                              statusCode: StatusCodes.Status403Forbidden);
                    */

                    throw new Exception();
                }
            }
            else
            {
                firstDate = new DateTime(firstWeekDay.Year, firstWeekDay.Month, 1);

                int daysInMonth = DateTime.DaysInMonth(firstWeekDay.Month, month);
                lastDate = new DateTime(firstWeekDay.Year, firstWeekDay.Month, daysInMonth);
            }

            var userId = timeSheet.User.Id;

            var group = await _context.MonthEntryGroups
               .Include(meg => meg.Entries)
               .FirstOrDefaultAsync(meg =>
                   meg.User.Id == userId
                   && meg.Year == lastDate.Date.Year
                   && meg.Month == lastDate.Date.Month);

            if (group is not null)
            {
                if (group.Status == EntryStatus.Locked)
                {
                    /*
                    return Problem(
                              title: "Unable to lock month",
                              detail: $"Month is already locked.",
                              statusCode: StatusCodes.Status403Forbidden);
                    */

                    return Unit.Value;
                }

                group.Status = EntryStatus.Locked;

                foreach (var entry in group.Entries)
                {
                    entry.Status = EntryStatus.Locked;
                }

                await _context.SaveChangesAsync();
            }

            return Unit.Value;
        }
    }
}