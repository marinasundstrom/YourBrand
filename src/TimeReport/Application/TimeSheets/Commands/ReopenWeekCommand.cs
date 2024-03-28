
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.TimeReport.Application.Common.Interfaces;
using YourBrand.TimeReport.Domain;
using YourBrand.TimeReport.Domain.Entities;
using YourBrand.TimeReport.Domain.Exceptions;
using YourBrand.TimeReport.Domain.Repositories;

namespace YourBrand.TimeReport.Application.TimeSheets.Commands;

public record ReopenWeekCommand(string TimeSheetId) : IRequest
{
    public class ReopenWeekCommandHandler : IRequestHandler<ReopenWeekCommand>
    {
        private readonly ITimeSheetRepository _timeSheetRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITimeReportContext _context;

        public ReopenWeekCommandHandler(ITimeSheetRepository timeSheetRepository, IUnitOfWork unitOfWork, ITimeReportContext context)
        {
            _timeSheetRepository = timeSheetRepository;
            _unitOfWork = unitOfWork;
            _context = context;
        }

        public async Task Handle(ReopenWeekCommand request, CancellationToken cancellationToken)
        {
            var timeSheet = await _timeSheetRepository.GetTimeSheet(request.TimeSheetId, cancellationToken);

            if (timeSheet is null)
            {
                throw new TimeSheetNotFoundException(request.TimeSheetId);
            }

            timeSheet.Reopen();

            await _unitOfWork.SaveChangesAsync(cancellationToken);

        }
    }
}
