using MediatR;

using YourBrand.TimeReport.Application.Common.Interfaces;
using YourBrand.TimeReport.Domain;
using YourBrand.TimeReport.Domain.Entities;
using YourBrand.TimeReport.Domain.Exceptions;
using YourBrand.TimeReport.Domain.Repositories;

namespace YourBrand.TimeReport.Application.TimeSheets.Commands;

public record UpdateEntryDetailsCommand(string OrganizationId, string TimeSheetId, string EntryId, string? Description) : IRequest<EntryDto>
{
    public class UpdateEntryDetailsCommandHandler(ITimeSheetRepository timeSheetRepository, IUnitOfWork unitOfWork, ITimeReportContext context) : IRequestHandler<UpdateEntryDetailsCommand, EntryDto>
    {
        public async Task<EntryDto> Handle(UpdateEntryDetailsCommand request, CancellationToken cancellationToken)
        {
            var timeSheet = await timeSheetRepository.GetTimeSheet(request.TimeSheetId, cancellationToken);

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

            entry.Description = request.Description;

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return entry.ToDto();
        }
    }
}