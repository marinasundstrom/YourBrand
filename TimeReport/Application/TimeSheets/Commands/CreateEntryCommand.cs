using System;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.TimeReport.Application.Activities;
using YourBrand.TimeReport.Application.Common.Interfaces;
using YourBrand.TimeReport.Application.Projects;
using YourBrand.TimeReport.Domain.Entities;
using YourBrand.TimeReport.Domain.Exceptions;

using static YourBrand.TimeReport.Application.TimeSheets.Constants;

namespace YourBrand.TimeReport.Application.TimeSheets.Commands;

public record CreateEntryCommand(string TimeSheetId, string ProjectId, string ActivityId, DateOnly Date, double? Hours, string? Description) : IRequest<ResultWithValue<EntryDto, Exception>>
{
    public class CreateEntryCommandHandler : IRequestHandler<CreateEntryCommand, ResultWithValue<EntryDto, Exception>>
    {
        private readonly ITimeReportContext _context;
        private readonly ICurrentUserService _currentUserService;

        public CreateEntryCommandHandler(ITimeReportContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<ResultWithValue<EntryDto, Exception>> Handle(CreateEntryCommand request, CancellationToken cancellationToken)
        {
            var timeSheet = await _context.TimeSheets.GetTimeSheetAsync(request.TimeSheetId, cancellationToken);

            if (timeSheet is null)
            {
                return new ResultWithValue<EntryDto, Exception>.Error(new TimeSheetNotFoundException(request.TimeSheetId));
            }

            if (timeSheet.Status != TimeSheetStatus.Open)
            {
                return new ResultWithValue<EntryDto, Exception>.Error(new TimeSheetClosedException(request.TimeSheetId));
            }

            var group = await _context.MonthEntryGroups.GetMonthGroup(timeSheet.UserId, request.Date.Year, request.Date.Month, cancellationToken);

            if (group is null)
            {
                group = new MonthEntryGroup(timeSheet.User, request.Date.Year, request.Date.Month);

                _context.MonthEntryGroups.Add(group);
            }
            else
            {
                if (group.Status == EntryStatus.Locked)
                {
                    return new ResultWithValue<EntryDto, Exception>.Error(new MonthLockedException(request.TimeSheetId));
                }
            }

            var date = request.Date;

            var existingEntryWithDate = timeSheet.Entries
                .FirstOrDefault(e => e.Date == date && e.Project.Id == request.ProjectId && e.Activity.Id == request.ActivityId);

            if (existingEntryWithDate is not null)
            {
                return new ResultWithValue<EntryDto, Exception>.Error(new EntryAlreadyExistsException(request.TimeSheetId, date, request.ActivityId));
            }

            var project = await _context.Projects
                .Include(x => x.Activities)
                .ThenInclude(x => x.ActivityType)
                .FirstOrDefaultAsync(x => x.Id == request.ProjectId, cancellationToken);

            if (project is null)
            {
                return new ResultWithValue<EntryDto, Exception>.Error(new ProjectNotFoundException(request.ProjectId));
            }

            var activity = project!.Activities.FirstOrDefault(x => x.Id == request.ActivityId);

            if (activity is null)
            {
                return new ResultWithValue<EntryDto, Exception>.Error(new ActivityNotFoundException(request.ProjectId));
            }

            var dateOnly = request.Date;

            double totalHoursDay = timeSheet.GetTotalHoursForDate(dateOnly)
                + request.Hours.GetValueOrDefault();

            if (totalHoursDay > WorkingDayHours)
            {
                return new ResultWithValue<EntryDto, Exception>.Error(new DayHoursExceedPermittedDailyWorkingHoursException(request.TimeSheetId, dateOnly));
            }

            double totalHoursWeek = timeSheet.GetTotalHours() + request.Hours.GetValueOrDefault();

            if (totalHoursWeek > WorkingWeekHours)
            {
                return new ResultWithValue<EntryDto, Exception>.Error(new WeekHoursExceedPermittedWeeklyWorkingHoursException(request.TimeSheetId));
            }

            var timeSheetActivity = timeSheet.GetActivity(activity);

            if (timeSheetActivity is null)
            {
                timeSheetActivity = timeSheet.AddActivity(activity);
            }

            var entry = timeSheetActivity.AddEntry(request.Date, request.Hours, request.Description);

            //entry.MonthGroup = group;

            group.AddEntry(entry);

            await _context.SaveChangesAsync(cancellationToken);

            var e = entry;

            return new ResultWithValue<EntryDto, Exception>.Ok(new EntryDto(e.Id, new ProjectDto(e.Project.Id, e.Project.Name, e.Project.Description), new ActivityDto(e.Activity.Id, e.Activity.Name, e.Activity.ActivityType.ToDto(), e.Activity.Description, e.Activity.HourlyRate, new ProjectDto(e.Activity.Project.Id, e.Activity.Project.Name, e.Activity.Project.Description)), e.Date.ToDateTime(TimeOnly.Parse("01:00")), e.Hours, e.Description, (EntryStatusDto)e.MonthGroup.Status));
        }
    }
}