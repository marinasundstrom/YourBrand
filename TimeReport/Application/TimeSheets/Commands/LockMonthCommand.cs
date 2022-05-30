
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.TimeReport.Application.Common.Interfaces;
using YourBrand.TimeReport.Domain.Entities;
using YourBrand.TimeReport.Domain.Exceptions;

namespace YourBrand.TimeReport.Application.TimeSheets.Commands;

public record LockMonthCommand(string TimeSheetId) : IRequest
{
    public class LockMonthCommandHandler : IRequestHandler<LockMonthCommand>
    {
        private readonly ITimeReportContext _context;

        public LockMonthCommandHandler(ITimeReportContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(LockMonthCommand request, CancellationToken cancellationToken)
        {
            var timeSheet = await _context.TimeSheets.GetTimeSheetAsync(request.TimeSheetId, cancellationToken);

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

            var group = await _context.MonthEntryGroups.GetMonthGroup(userId, lastDate.Date.Year, lastDate.Date.Month, cancellationToken);

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

                // Cannot lock month with open Timesheets

                var hasTimeSheetsOpen = group.Entries
                    .Select(x => x.TimeSheet)
                    .Distinct()
                    .Any(x => x.Status == TimeSheetStatus.Open);

                if(hasTimeSheetsOpen) 
                {
                    throw new Exception("Cannot lock month since timesheets are open.");
                }

                group.UpdateStatus(EntryStatus.Locked);

                foreach (var entry in group.Entries)
                {
                    entry.Lock();
                }

                await _context.SaveChangesAsync();
            }

            return Unit.Value;
        }
    }
}