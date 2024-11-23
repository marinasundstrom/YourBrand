
using MediatR;

using YourBrand.TimeReport.Domain;
using YourBrand.TimeReport.Domain.Repositories;

namespace YourBrand.TimeReport.Application.TimeSheets.Commands;

public record ApproveWeekCommand(string OrganizationId, string TimeSheetId) : IRequest<Result>
{
    public class ApproveWeekCommandHandler(ITimeSheetRepository timeSheetRepository, IUnitOfWork unitOfWork) : IRequestHandler<ApproveWeekCommand, Result>
    {
        public async Task<Result> Handle(ApproveWeekCommand request, CancellationToken cancellationToken)
        {
            var timeSheet = await timeSheetRepository.GetTimeSheet(request.TimeSheetId, cancellationToken);

            if (timeSheet is null)
            {
                return new TimeSheetNotFound(request.TimeSheetId);
            }

            timeSheet.Approve();

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success;
        }
    }
}