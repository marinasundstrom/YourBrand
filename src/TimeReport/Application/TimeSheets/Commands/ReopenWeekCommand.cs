
using MediatR;

using YourBrand.TimeReport.Application.Common.Interfaces;
using YourBrand.TimeReport.Domain;
using YourBrand.TimeReport.Domain.Exceptions;
using YourBrand.TimeReport.Domain.Repositories;

namespace YourBrand.TimeReport.Application.TimeSheets.Commands;

public record ReopenWeekCommand(string OrganizationId, string TimeSheetId) : IRequest
{
    public class ReopenWeekCommandHandler(ITimeSheetRepository timeSheetRepository, IUnitOfWork unitOfWork, ITimeReportContext context) : IRequestHandler<ReopenWeekCommand>
    {
        public async Task Handle(ReopenWeekCommand request, CancellationToken cancellationToken)
        {
            var timeSheet = await timeSheetRepository.GetTimeSheet(request.TimeSheetId, cancellationToken);

            if (timeSheet is null)
            {
                throw new TimeSheetNotFoundException(request.TimeSheetId);
            }

            timeSheet.Reopen();

            await unitOfWork.SaveChangesAsync(cancellationToken);

        }
    }
}