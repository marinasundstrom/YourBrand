
using MediatR;

using YourBrand.TimeReport.Application.Common.Interfaces;
using YourBrand.TimeReport.Domain;
using YourBrand.TimeReport.Domain.Repositories;

namespace YourBrand.TimeReport.Application.TimeSheets.Queries;

public record GetTimeSheetQuery(string OrganizationId, string TimeSheetId) : IRequest<TimeSheetDto?>
{
    public class GetTimeSheetQueryHandler(ITimeSheetRepository timeSheetRepository, IReportingPeriodRepository reportingPeriodRepository, IUnitOfWork unitOfWork, ITimeReportContext context) : IRequestHandler<GetTimeSheetQuery, TimeSheetDto?>
    {
        public async Task<TimeSheetDto?> Handle(GetTimeSheetQuery request, CancellationToken cancellationToken)
        {
            var timeSheet = await timeSheetRepository.GetTimeSheet(request.TimeSheetId, cancellationToken);

            if (timeSheet is null)
            {
                return null;
            }

            var periods = await reportingPeriodRepository.GetReportingPeriodForTimeSheet(timeSheet, cancellationToken);

            return timeSheet.ToDto(periods);
        }
    }
}