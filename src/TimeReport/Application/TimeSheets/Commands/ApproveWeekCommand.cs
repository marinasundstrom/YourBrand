
using MediatR;

using YourBrand.TimeReport.Domain;
using YourBrand.TimeReport.Domain.Exceptions;
using YourBrand.TimeReport.Domain.Repositories;

namespace YourBrand.TimeReport.Application.TimeSheets.Commands;

public record ApproveWeekCommand(string OrganizationId, string TimeSheetId) : IRequest
{
    public class ApproveWeekCommandHandler(ITimeSheetRepository timeSheetRepository, IUnitOfWork unitOfWork) : IRequestHandler<ApproveWeekCommand>
    {
        public async Task Handle(ApproveWeekCommand request, CancellationToken cancellationToken)
        {
            var timeSheet = await timeSheetRepository.GetTimeSheet(request.TimeSheetId, cancellationToken);

            if (timeSheet is null)
            {
                throw new TimeSheetNotFoundException(request.TimeSheetId);
            }

            timeSheet.Approve();

            await unitOfWork.SaveChangesAsync(cancellationToken);

        }
    }
}