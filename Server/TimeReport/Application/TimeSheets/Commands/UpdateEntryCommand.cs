
using MediatR;

using Microsoft.EntityFrameworkCore;

using TimeReport.Application.Activities;
using TimeReport.Application.Common.Interfaces;
using TimeReport.Application.Projects;
using TimeReport.Domain.Entities;
using TimeReport.Domain.Exceptions;

using static TimeReport.Application.TimeSheets.Constants;

namespace TimeReport.Application.TimeSheets.Commands;

public class UpdateEntryCommand : IRequest<EntryDto>
{
    public UpdateEntryCommand(string timeSheetId, string entryId, double? hours, string? description)
    {
        TimeSheetId = timeSheetId;
        EntryId = entryId;
        Hours = hours;
        Description = description;
    }

    public string TimeSheetId { get; }

    public string EntryId { get; }

    public double? Hours { get; }

    public string? Description { get; }

    public class UpdateEntryCommandHandler : IRequestHandler<UpdateEntryCommand, EntryDto>
    {
        private readonly ITimeReportContext _context;

        public UpdateEntryCommandHandler(ITimeReportContext context)
        {
            _context = context;
        }

        public async Task<EntryDto> Handle(UpdateEntryCommand request, CancellationToken cancellationToken)
        {
            var timeSheet = await _context.TimeSheets
                        .Include(x => x.Entries)
                        .ThenInclude(x => x.MonthGroup)
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

            if (timeSheet.Status != TimeSheetStatus.Open)
            {
                throw new TimeSheetClosedException(request.TimeSheetId);
            }

            var entry = timeSheet.Entries.FirstOrDefault(e => e.Id == request.EntryId);

            if (entry is null)
            {
                throw new EntryNotFoundException(request.EntryId);
            }

            if (entry.MonthGroup.Status == EntryStatus.Locked)
            {
                throw new MonthLockedException(request.TimeSheetId);
            }

            entry.UpdateHours(request.Hours);
            entry.Description = request.Description;

            double totalHoursDay = timeSheet.Entries.Where(e => e.Date == entry.Date).Sum(e => e.Hours.GetValueOrDefault());
            if (totalHoursDay > WorkingDayHours)
            {
                throw new DayHoursExceedPermittedDailyWorkingHoursException(request.TimeSheetId, entry.Date);
            }

            double totalHoursWeek = timeSheet.Entries.Sum(x => x.Hours.GetValueOrDefault());
            if (totalHoursWeek > WorkingWeekHours)
            {
                throw new WeekHoursExceedPermittedWeeklyWorkingHoursException(request.TimeSheetId);
            }

            await _context.SaveChangesAsync(cancellationToken);

            var e = entry;

            return new EntryDto(e.Id, new ProjectDto(e.Project.Id, e.Project.Name, e.Project.Description), new ActivityDto(e.Activity.Id, e.Activity.Name, e.Activity.Description, e.Activity.HourlyRate, new ProjectDto(e.Activity.Project.Id, e.Activity.Project.Name, e.Activity.Project.Description)), e.Date.ToDateTime(TimeOnly.Parse("01:00")), e.Hours, e.Description, (EntryStatusDto)e.MonthGroup.Status);
        }
    }
}