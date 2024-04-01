using MediatR;

using YourBrand.TimeReport.Application.Common.Interfaces;
using YourBrand.TimeReport.Domain;
using YourBrand.TimeReport.Domain.Entities;
using YourBrand.TimeReport.Domain.Exceptions;
using YourBrand.TimeReport.Domain.Repositories;

namespace YourBrand.TimeReport.Application.TimeSheets.Commands;

public record UpdateEntryDetailsCommand(string TimeSheetId, string EntryId, string? Description) : IRequest<EntryDto>
{
    public class UpdateEntryDetailsCommandHandler : IRequestHandler<UpdateEntryDetailsCommand, EntryDto>
    {
        private readonly ITimeSheetRepository _timeSheetRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITimeReportContext _context;

        public UpdateEntryDetailsCommandHandler(ITimeSheetRepository timeSheetRepository, IUnitOfWork unitOfWork, ITimeReportContext context)
        {
            _timeSheetRepository = timeSheetRepository;
            _unitOfWork = unitOfWork;
            _context = context;
        }

        public async Task<EntryDto> Handle(UpdateEntryDetailsCommand request, CancellationToken cancellationToken)
        {
            var timeSheet = await _timeSheetRepository.GetTimeSheet(request.TimeSheetId, cancellationToken);

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

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return entry.ToDto();
        }
    }
}