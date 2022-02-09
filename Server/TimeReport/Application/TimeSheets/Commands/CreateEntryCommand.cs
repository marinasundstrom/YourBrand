using System;

using MediatR;

using Microsoft.EntityFrameworkCore;

using TimeReport.Application.Activities;
using TimeReport.Application.Common.Interfaces;
using TimeReport.Application.Projects;
using TimeReport.Domain.Entities;
using TimeReport.Domain.Exceptions;

using static TimeReport.Application.TimeSheets.Constants;

namespace TimeReport.Application.TimeSheets.Commands;

public class CreateEntryCommand : IRequest<EntryDto>
{
    public CreateEntryCommand(string timeSheetId, string projectId, string activityId, DateOnly date, double? hours, string? description)
    {
        TimeSheetId = timeSheetId;
        ProjectId = projectId;
        ActivityId = activityId;
        Date = date;
        Hours = hours;
        Description = description;
    }

    public string ProjectId { get; }

    public string ActivityId { get; }

    public string TimeSheetId { get; }

    public DateOnly Date { get; }

    public double? Hours { get; }

    public string? Description { get; }

    public class CreateEntryCommandHandler : IRequestHandler<CreateEntryCommand, EntryDto>
    {
        private readonly ITimeReportContext _context;

        public CreateEntryCommandHandler(ITimeReportContext context)
        {
            _context = context;
        }

        public async Task<EntryDto> Handle(CreateEntryCommand request, CancellationToken cancellationToken)
        {
            var timeSheet = await _context.TimeSheets
               .Include(x => x.User)
               .Include(x => x.Entries)
               .ThenInclude(x => x.Project)
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

            var group = await _context.MonthEntryGroups
                .FirstOrDefaultAsync(meg =>
                    meg.User.Id == timeSheet.User.Id
                    && meg.Year == request.Date.Year
                    && meg.Month == request.Date.Month, cancellationToken);

            if (group is null)
            {
                group = new MonthEntryGroup
                {
                    Id = Guid.NewGuid().ToString(),
                    User = timeSheet.User,
                    Year = request.Date.Year,
                    Month = request.Date.Month,
                    Status = EntryStatus.Unlocked
                };

                _context.MonthEntryGroups.Add(group);
            }
            else
            {
                if (group.Status == EntryStatus.Locked)
                {
                    throw new MonthLockedException(request.TimeSheetId); ;
                }
            }

            var date = request.Date;

            var existingEntryWithDate = timeSheet.Entries
                .FirstOrDefault(e => e.Date == date && e.Project.Id == request.ProjectId && e.Activity.Id == request.ActivityId);

            if (existingEntryWithDate is not null)
            {
                throw new EntryAlreadyExistsException(request.TimeSheetId, date, request.ActivityId);
            }

            var project = await _context.Projects
                .Include(x => x.Activities)
                .FirstOrDefaultAsync(x => x.Id == request.ProjectId, cancellationToken);

            if (project is null)
            {
                throw new ProjectNotFoundException(request.ProjectId);
            }

            var activity = project!.Activities.FirstOrDefault(x => x.Id == request.ActivityId);

            if (activity is null)
            {
                throw new ActivityNotFoundException(request.ProjectId);
            }

            var dateOnly = request.Date;

            double totalHoursDay = timeSheet.Entries.Where(e => e.Date == dateOnly).Sum(e => e.Hours.GetValueOrDefault())
                + request.Hours.GetValueOrDefault();

            if (totalHoursDay > WorkingDayHours)
            {
                throw new DayHoursExceedPermittedDailyWorkingHoursException(request.TimeSheetId, dateOnly);
            }

            double totalHoursWeek = timeSheet.Entries.Sum(x => x.Hours.GetValueOrDefault())
                + request.Hours.GetValueOrDefault();

            if (totalHoursWeek > WorkingWeekHours)
            {
                throw new WeekHoursExceedPermittedWeeklyWorkingHoursException(request.TimeSheetId);
            }

            var timeSheetActivity = await _context.TimeSheetActivities
                .FirstOrDefaultAsync(x => x.TimeSheet.Id == timeSheet.Id && x.Activity.Id == activity.Id, cancellationToken);

            if (timeSheetActivity is null)
            {
                timeSheetActivity = new TimeSheetActivity
                {
                    Id = Guid.NewGuid().ToString(),
                    TimeSheet = timeSheet,
                    Project = project,
                    Activity = activity
                };

                _context.TimeSheetActivities.Add(timeSheetActivity);
            }

            var entry = new Entry
            {
                Id = Guid.NewGuid().ToString(),
                User = timeSheet.User,
                Project = project,
                Activity = activity,
                TimeSheetActivity = timeSheetActivity,
                Date = request.Date,
                Hours = request.Hours,
                Description = request.Description
            };

            timeSheet.Entries.Add(entry);

            entry.MonthGroup = group;

            group.Entries.Add(entry);

            await _context.SaveChangesAsync(cancellationToken);

            var e = entry;

            return new EntryDto(e.Id, new ProjectDto(e.Project.Id, e.Project.Name, e.Project.Description), new ActivityDto(e.Activity.Id, e.Activity.Name, e.Activity.Description, e.Activity.HourlyRate, new ProjectDto(e.Activity.Project.Id, e.Activity.Project.Name, e.Activity.Project.Description)), e.Date.ToDateTime(TimeOnly.Parse("01:00")), e.Hours, e.Description, (EntryStatusDto)e.MonthGroup.Status);
        }
    }
}