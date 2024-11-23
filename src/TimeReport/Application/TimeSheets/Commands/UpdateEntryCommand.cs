
using MediatR;

using YourBrand.TimeReport.Application.Common.Interfaces;
using YourBrand.TimeReport.Domain;
using YourBrand.TimeReport.Domain.Entities;
using YourBrand.TimeReport.Domain.Repositories;

using static YourBrand.TimeReport.Application.TimeSheets.Constants;

namespace YourBrand.TimeReport.Application.TimeSheets.Commands;

public record UpdateEntryCommand(string OrganizationId, string TimeSheetId, string EntryId, double? Hours, string? Description) : IRequest<Result<EntryDto>>
{
    public class UpdateEntryCommandHandler(ITimeSheetRepository timeSheetRepository, IUnitOfWork unitOfWork, ITimeReportContext context) : IRequestHandler<UpdateEntryCommand, Result<EntryDto>>
    {
        public async Task<Result<EntryDto>> Handle(UpdateEntryCommand request, CancellationToken cancellationToken)
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

            var entry = timeSheet.Entries.FirstOrDefault(e => e.Id == request.EntryId);

            if (entry is null)
            {
                return new EntryNotFound(request.EntryId);
            }

            if (entry.MonthGroup.Status == EntryStatus.Locked)
            {
                return new MonthLocked(request.TimeSheetId);
            }

            entry.UpdateHours(request.Hours);
            entry.Description = request.Description;

            double totalHoursDay = timeSheet.Entries.Where(e => e.Date == entry.Date).Sum(e => e.Hours.GetValueOrDefault());
            if (totalHoursDay > WorkingDayHours)
            {
                return new DayHoursExceedPermittedDailyWorkingHours(request.TimeSheetId, entry.Date);
            }

            double totalHoursWeek = timeSheet.Entries.Sum(x => x.Hours.GetValueOrDefault());
            if (totalHoursWeek > WorkingWeekHours)
            {
                return new WeekHoursExceedPermittedWeeklyWorkingHours(request.TimeSheetId);
            }

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return entry.ToDto();
        }
    }
}