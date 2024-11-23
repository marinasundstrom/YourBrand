
using MediatR;

using YourBrand.TimeReport.Domain;
using YourBrand.TimeReport.Domain.Repositories;

namespace YourBrand.TimeReport.Application.TimeSheets.Commands;

public record CloseWeekCommand(string OrganizationId, string TimeSheetId) : IRequest<Result>
{
    public class CloseWeekCommandHandler(ITimeSheetRepository timeSheetRepository, IUnitOfWork unitOfWork) : IRequestHandler<CloseWeekCommand, Result>
    {
        public async Task<Result> Handle(CloseWeekCommand request, CancellationToken cancellationToken)
        {
            var timeSheet = await timeSheetRepository.GetTimeSheet(request.TimeSheetId, cancellationToken);

            if (timeSheet is null)
            {
                return new TimeSheetNotFound(request.TimeSheetId);
            }

            timeSheet.Close();

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success;
        }
    }
}