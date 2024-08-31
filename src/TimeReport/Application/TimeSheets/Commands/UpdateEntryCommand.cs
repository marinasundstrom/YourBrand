
using MediatR;

using YourBrand.TimeReport.Application.Common.Interfaces;
using YourBrand.TimeReport.Domain;
using YourBrand.TimeReport.Domain.Entities;
using YourBrand.TimeReport.Domain.Exceptions;
using YourBrand.TimeReport.Domain.Repositories;

using static YourBrand.TimeReport.Application.TimeSheets.Constants;

namespace YourBrand.TimeReport.Application.TimeSheets.Commands;

public record UpdateEntryCommand(string OrganizationId, string TimeSheetId, string EntryId, double? Hours, string? Description) : IRequest<Result<EntryDto, DomainException>>
{
    public class UpdateEntryCommandHandler(ITimeSheetRepository timeSheetRepository, IUnitOfWork unitOfWork, ITimeReportContext context) : IRequestHandler<UpdateEntryCommand, Result<EntryDto, DomainException>>
    {
        public async Task<Result<EntryDto, DomainException>> Handle(UpdateEntryCommand request, CancellationToken cancellationToken)
        {
            var timeSheet = await timeSheetRepository.GetTimeSheet(request.TimeSheetId, cancellationToken);

            if (timeSheet is null)
            {
                return new Result<EntryDto, DomainException>.Error(new TimeSheetNotFoundException(request.TimeSheetId));
            }

            if (timeSheet.Status != TimeSheetStatus.Open)
            {
                return new Result<EntryDto, DomainException>.Error(new TimeSheetClosedException(request.TimeSheetId));
            }

            var entry = timeSheet.Entries.FirstOrDefault(e => e.Id == request.EntryId);

            if (entry is null)
            {
                return new Result<EntryDto, DomainException>.Error(new EntryNotFoundException(request.EntryId));
            }

            if (entry.MonthGroup.Status == EntryStatus.Locked)
            {
                return new Result<EntryDto, DomainException>.Error(new MonthLockedException(request.TimeSheetId));
            }

            entry.UpdateHours(request.Hours);
            entry.Description = request.Description;

            double totalHoursDay = timeSheet.Entries.Where(e => e.Date == entry.Date).Sum(e => e.Hours.GetValueOrDefault());
            if (totalHoursDay > WorkingDayHours)
            {
                return new Result<EntryDto, DomainException>.Error(new DayHoursExceedPermittedDailyWorkingHoursException(request.TimeSheetId, entry.Date));
            }

            double totalHoursWeek = timeSheet.Entries.Sum(x => x.Hours.GetValueOrDefault());
            if (totalHoursWeek > WorkingWeekHours)
            {
                return new Result<EntryDto, DomainException>.Error(new WeekHoursExceedPermittedWeeklyWorkingHoursException(request.TimeSheetId));
            }

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return new Result<EntryDto, DomainException>.Ok(entry.ToDto());
        }
    }
}