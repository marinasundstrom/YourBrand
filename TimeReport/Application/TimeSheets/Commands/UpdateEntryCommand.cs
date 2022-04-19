
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.TimeReport.Application.Activities;
using YourBrand.TimeReport.Application.Common.Interfaces;
using YourBrand.TimeReport.Application.Projects;
using YourBrand.TimeReport.Domain.Entities;
using YourBrand.TimeReport.Domain.Exceptions;

using static YourBrand.TimeReport.Application.TimeSheets.Constants;

namespace YourBrand.TimeReport.Application.TimeSheets.Commands;

public record UpdateEntryCommand(string TimeSheetId, string EntryId, double? Hours, string? Description) : IRequest<ResultWithValue<EntryDto, DomainException>>
{
    public class UpdateEntryCommandHandler : IRequestHandler<UpdateEntryCommand, ResultWithValue<EntryDto, DomainException>>
    {
        private readonly ITimeReportContext _context;

        public UpdateEntryCommandHandler(ITimeReportContext context)
        {
            _context = context;
        }

        public async Task<ResultWithValue<EntryDto, DomainException>> Handle(UpdateEntryCommand request, CancellationToken cancellationToken)
        {
            var timeSheet = await _context.TimeSheets.GetTimeSheetAsync(request.TimeSheetId, cancellationToken);

            if (timeSheet is null)
            {
                return new ResultWithValue<EntryDto, DomainException>.Error(new TimeSheetNotFoundException(request.TimeSheetId));
            }

            if (timeSheet.Status != TimeSheetStatus.Open)
            {
                return new ResultWithValue<EntryDto, DomainException>.Error(new TimeSheetClosedException(request.TimeSheetId));
            }

            var entry = timeSheet.Entries.FirstOrDefault(e => e.Id == request.EntryId);

            if (entry is null)
            {
                return new ResultWithValue<EntryDto, DomainException>.Error(new EntryNotFoundException(request.EntryId));
            }

            if (entry.MonthGroup.Status == EntryStatus.Locked)
            {
                return new ResultWithValue<EntryDto, DomainException>.Error(new MonthLockedException(request.TimeSheetId));
            }

            entry.UpdateHours(request.Hours);
            entry.Description = request.Description;

            double totalHoursDay = timeSheet.Entries.Where(e => e.Date == entry.Date).Sum(e => e.Hours.GetValueOrDefault());
            if (totalHoursDay > WorkingDayHours)
            {
                return new ResultWithValue<EntryDto, DomainException>.Error(new DayHoursExceedPermittedDailyWorkingHoursException(request.TimeSheetId, entry.Date));
            }

            double totalHoursWeek = timeSheet.Entries.Sum(x => x.Hours.GetValueOrDefault());
            if (totalHoursWeek > WorkingWeekHours)
            {
                return new ResultWithValue<EntryDto, DomainException>.Error(new WeekHoursExceedPermittedWeeklyWorkingHoursException(request.TimeSheetId));
            }

            await _context.SaveChangesAsync(cancellationToken);

            return new ResultWithValue<EntryDto, DomainException>.Ok(entry.ToDto());
        }
    }
}