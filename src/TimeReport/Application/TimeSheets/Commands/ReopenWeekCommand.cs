
using MediatR;

using YourBrand.TimeReport.Application.Common.Interfaces;
using YourBrand.TimeReport.Domain;
using YourBrand.TimeReport.Domain.Repositories;

namespace YourBrand.TimeReport.Application.TimeSheets.Commands;

public record ReopenWeekCommand(string OrganizationId, string TimeSheetId) : IRequest<Result>
{
    public class ReopenWeekCommandHandler(ITimeSheetRepository timeSheetRepository, IUnitOfWork unitOfWork, ITimeReportContext context) : IRequestHandler<ReopenWeekCommand, Result>
    {
        public async Task<Result> Handle(ReopenWeekCommand request, CancellationToken cancellationToken)
        {
            var timeSheet = await timeSheetRepository.GetTimeSheet(request.TimeSheetId, cancellationToken);

            if (timeSheet is null)
            {
                return new TimeSheetNotFound(request.TimeSheetId);
            }

            timeSheet.Reopen();

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success;
        }
    }
}