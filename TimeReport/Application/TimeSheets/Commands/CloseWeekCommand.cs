
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.TimeReport.Application.Common.Interfaces;
using YourBrand.TimeReport.Domain;
using YourBrand.TimeReport.Domain.Entities;
using YourBrand.TimeReport.Domain.Exceptions;
using YourBrand.TimeReport.Domain.Repositories;

namespace YourBrand.TimeReport.Application.TimeSheets.Commands;

public record CloseWeekCommand(string TimeSheetId) : IRequest
{
    public class CloseWeekCommandHandler : IRequestHandler<CloseWeekCommand>
    {
        private readonly ITimeSheetRepository _timeSheetRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CloseWeekCommandHandler(ITimeSheetRepository timeSheetRepository, IUnitOfWork unitOfWork)
        {
            _timeSheetRepository = timeSheetRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(CloseWeekCommand request, CancellationToken cancellationToken)
        {
            var timeSheet = await _timeSheetRepository.GetTimeSheet(request.TimeSheetId, cancellationToken);

            if (timeSheet is null)
            {
                throw new TimeSheetNotFoundException(request.TimeSheetId);
            }

            timeSheet.Close();

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
