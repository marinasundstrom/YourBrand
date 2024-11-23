using MediatR;

using YourBrand.TimeReport.Application.Common.Interfaces;
using YourBrand.TimeReport.Domain;
using YourBrand.TimeReport.Domain.Entities;
using YourBrand.TimeReport.Domain.Repositories;

namespace YourBrand.TimeReport.Application.TimeSheets.Commands;

public record UpdateEntryDetailsCommand(string OrganizationId, string TimeSheetId, string EntryId, string? Description) : IRequest<Result<EntryDto>>
{
    public class UpdateEntryDetailsCommandHandler(ITimeSheetRepository timeSheetRepository, IUnitOfWork unitOfWork, ITimeReportContext context) : IRequestHandler<UpdateEntryDetailsCommand, Result<EntryDto>>
    {
        public async Task<Result<EntryDto>> Handle(UpdateEntryDetailsCommand request, CancellationToken cancellationToken)
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

            entry.Description = request.Description;

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return entry.ToDto();
        }
    }
}