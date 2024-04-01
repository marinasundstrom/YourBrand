
using MediatR;

using YourBrand.TimeReport.Domain;
using YourBrand.TimeReport.Domain.Exceptions;
using YourBrand.TimeReport.Domain.Repositories;

namespace YourBrand.TimeReport.Application.TimeSheets.Commands;

public record ApproveWeekCommand(string TimeSheetId) : IRequest
{
    public class ApproveWeekCommandHandler : IRequestHandler<ApproveWeekCommand>
    {
        private readonly ITimeSheetRepository _timeSheetRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ApproveWeekCommandHandler(ITimeSheetRepository timeSheetRepository, IUnitOfWork unitOfWork)
        {
            _timeSheetRepository = timeSheetRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(ApproveWeekCommand request, CancellationToken cancellationToken)
        {
            var timeSheet = await _timeSheetRepository.GetTimeSheet(request.TimeSheetId, cancellationToken);

            if (timeSheet is null)
            {
                throw new TimeSheetNotFoundException(request.TimeSheetId);
            }

            timeSheet.Approve();

            await _unitOfWork.SaveChangesAsync(cancellationToken);

        }
    }
}