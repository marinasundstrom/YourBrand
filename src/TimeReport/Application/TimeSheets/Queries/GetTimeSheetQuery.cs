
using MediatR;

using YourBrand.TimeReport.Application.Common.Interfaces;
using YourBrand.TimeReport.Domain;
using YourBrand.TimeReport.Domain.Repositories;

namespace YourBrand.TimeReport.Application.TimeSheets.Queries;

public record GetTimeSheetQuery(string TimeSheetId) : IRequest<TimeSheetDto?>
{
    public class GetTimeSheetQueryHandler : IRequestHandler<GetTimeSheetQuery, TimeSheetDto?>
    {
        private readonly ITimeSheetRepository _timeSheetRepository;
        private readonly IReportingPeriodRepository _reportingPeriodRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITimeReportContext _context;

        public GetTimeSheetQueryHandler(ITimeSheetRepository timeSheetRepository, IReportingPeriodRepository reportingPeriodRepository, IUnitOfWork unitOfWork, ITimeReportContext context)
        {
            _timeSheetRepository = timeSheetRepository;
            _reportingPeriodRepository = reportingPeriodRepository;
            _unitOfWork = unitOfWork;
            _context = context;
        }

        public async Task<TimeSheetDto?> Handle(GetTimeSheetQuery request, CancellationToken cancellationToken)
        {
            var timeSheet = await _timeSheetRepository.GetTimeSheet(request.TimeSheetId, cancellationToken);

            if (timeSheet is null)
            {
                return null;
            }

            var periods = await _reportingPeriodRepository.GetReportingPeriodForTimeSheet(timeSheet, cancellationToken);

            return timeSheet.ToDto(periods);
        }
    }
}