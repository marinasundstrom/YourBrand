
using MediatR;

using YourBrand.TimeReport.Application.Common.Interfaces;
using YourBrand.TimeReport.Domain;
using YourBrand.TimeReport.Domain.Exceptions;
using YourBrand.TimeReport.Domain.Repositories;

namespace YourBrand.TimeReport.Application.TimeSheets.Commands;

public record UpdateTimeSheetStatusCommand(string OrganizationId, string TimeSheetId) : IRequest
{
    public class UpdateTimeSheetStatusCommandHandler(ITimeSheetRepository timeSheetRepository, IUnitOfWork unitOfWork, ITimeReportContext context) : IRequestHandler<UpdateTimeSheetStatusCommand>
    {
        public async Task Handle(UpdateTimeSheetStatusCommand request, CancellationToken cancellationToken)
        {
            var timeSheet = await timeSheetRepository.GetTimeSheet(request.TimeSheetId, cancellationToken);

            if (timeSheet is null)
            {
                throw new TimeSheetNotFoundException(request.TimeSheetId);
            }

            timeSheet.Close();

            await unitOfWork.SaveChangesAsync();

        }
    }
}