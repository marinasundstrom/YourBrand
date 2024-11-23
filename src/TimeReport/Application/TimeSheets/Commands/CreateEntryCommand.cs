using MediatR;

using YourBrand.Identity;
using YourBrand.TimeReport.Application.Common.Interfaces;
using YourBrand.TimeReport.Domain;
using YourBrand.TimeReport.Domain.Entities;
using YourBrand.TimeReport.Domain.Repositories;

using static YourBrand.TimeReport.Application.TimeSheets.Constants;

namespace YourBrand.TimeReport.Application.TimeSheets.Commands;

public record CreateEntryCommand(string OrganizationId, string TimeSheetId, string ProjectId, string ActivityId, DateOnly Date, double? Hours, string? Description) : IRequest<Result<EntryDto>>
{
    public class CreateEntryCommandHandler(ITimeSheetRepository timeSheetRepository, IReportingPeriodRepository reportingPeriodRepository, IProjectRepository projectRepository, IUnitOfWork unitOfWork) : IRequestHandler<CreateEntryCommand, Result<EntryDto>>
    {
        public async Task<Result<EntryDto>> Handle(CreateEntryCommand request, CancellationToken cancellationToken)
        {
            var timeSheet = await timeSheetRepository.GetTimeSheet(request.TimeSheetId, cancellationToken);

            if (timeSheet is null)
            {
                return new TimeSheetNotFound(request.TimeSheetId);
            }

            if (timeSheet.Status != TimeSheetStatus.Open)
            {
                return new TimeSheetClosed(request.TimeSheetId);
            }

            var group = await reportingPeriodRepository.GetReportingPeriod(timeSheet.UserId, request.Date.Year, request.Date.Month, cancellationToken);

            if (group is null)
            {
                group = new ReportingPeriod(timeSheet.User, request.Date.Year, request.Date.Month);
                group.OrganizationId = request.OrganizationId;

                reportingPeriodRepository.AddReportingPeriod(group);
            }
            else
            {
                if (group.Status == EntryStatus.Locked)
                {
                    return new MonthLocked(request.TimeSheetId);
                }
            }

            var date = request.Date;

            var existingEntryWithDate = timeSheet.Entries
                .FirstOrDefault(e => e.Date == date && e.Project.Id == request.ProjectId && e.Activity.Id == request.ActivityId);

            if (existingEntryWithDate is not null)
            {
                return new EntryAlreadyExists(request.TimeSheetId, date, request.ActivityId);
            }

            var project = await projectRepository.GetProject(request.ProjectId, cancellationToken);

            if (project is null)
            {
                return new ProjectNotFound(request.ProjectId);
            }

            var activity = project!.Activities.FirstOrDefault(x => x.Id == request.ActivityId);

            if (activity is null)
            {
                return new ActivityNotFound(request.ProjectId);
            }

            var dateOnly = request.Date;

            double totalHoursDay = timeSheet.GetTotalHoursForDate(dateOnly)
                + request.Hours.GetValueOrDefault();

            if (totalHoursDay > WorkingDayHours)
            {
                return new DayHoursExceedPermittedDailyWorkingHours(request.TimeSheetId, dateOnly);
            }

            double totalHoursWeek = timeSheet.GetTotalHours() + request.Hours.GetValueOrDefault();

            if (totalHoursWeek > WorkingWeekHours)
            {
                return new WeekHoursExceedPermittedWeeklyWorkingHours(request.TimeSheetId);
            }

            var timeSheetActivity = timeSheet.GetActivity(activity.Id);

            if (timeSheetActivity is null)
            {
                timeSheetActivity = timeSheet.AddActivity(activity);
            }

            var entry = timeSheetActivity.AddEntry(request.Date, request.Hours, request.Description);

            group.AddEntry(entry);

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return entry.ToDto();
        }
    }
}