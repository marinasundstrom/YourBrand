
using MediatR;

using YourBrand.TimeReport.Application.Common.Interfaces;
using YourBrand.TimeReport.Domain;
using YourBrand.TimeReport.Domain.Repositories;

namespace YourBrand.TimeReport.Application.TimeSheets.Commands;

public record UpdateTimeSheetStatusCommand(string OrganizationId, string TimeSheetId) : IRequest<Result>
{
    public class UpdateTimeSheetStatusCommandHandler(ITimeSheetRepository timeSheetRepository, IUnitOfWork unitOfWork, ITimeReportContext context) : IRequestHandler<UpdateTimeSheetStatusCommand, Result>
    {
        public async Task<Result> Handle(UpdateTimeSheetStatusCommand request, CancellationToken cancellationToken)
        {
            var timeSheet = await timeSheetRepository.GetTimeSheet(request.TimeSheetId, cancellationToken);

            if (timeSheet is null)
            {
                return new TimeSheetNotFound(request.TimeSheetId);
            }

            timeSheet.Close();

            await unitOfWork.SaveChangesAsync();

            return Result.Success;
        }
    }
}