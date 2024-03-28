using System.Globalization;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;
using YourBrand.Tenancy;
using YourBrand.TimeReport.Application.Common.Interfaces;
using YourBrand.TimeReport.Application.Projects;
using YourBrand.TimeReport.Application.Users;
using YourBrand.TimeReport.Application.Users.Commands;
using YourBrand.TimeReport.Domain;
using YourBrand.TimeReport.Domain.Entities;
using YourBrand.TimeReport.Domain.Repositories;

namespace YourBrand.TimeReport.Application.TimeSheets.Queries;

public record GetTimeSheetForWeekQuery(int Year, int Week, string? UserId) : IRequest<TimeSheetDto?>
{
    public class GetTimeSheetForWeekQueryHandler : IRequestHandler<GetTimeSheetForWeekQuery, TimeSheetDto?>
    {
        private readonly ITimeSheetRepository _timeSheetRepository;
        private readonly IReportingPeriodRepository _reportingPeriodRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITimeReportContext _context;
        private readonly ICurrentUserService _currentUserService;

        public GetTimeSheetForWeekQueryHandler(ITimeSheetRepository timeSheetRepository, IReportingPeriodRepository reportingPeriodRepository, IUserRepository userRepository, IUnitOfWork unitOfWork, ITimeReportContext context, ICurrentUserService currentUserService)
        {
            _timeSheetRepository = timeSheetRepository;
            _reportingPeriodRepository = reportingPeriodRepository;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<TimeSheetDto?> Handle(GetTimeSheetForWeekQuery request, CancellationToken cancellationToken)
        {
            var query = _timeSheetRepository.GetTimeSheets()
                .AsSplitQuery();

            string? userId = request.UserId ?? _currentUserService.UserId;

            query = query.Where(x => x.UserId == userId);

            var timeSheet = await query.FirstOrDefaultAsync(x => x.Year == request.Year && x.Week == request.Week, cancellationToken);

            if (timeSheet is null)
            {
                User? user = await _userRepository.GetUser(userId!, cancellationToken);
                
                userId = user?.Id;

                var startDate = ISOWeek.ToDateTime(request.Year, request.Week, DayOfWeek.Monday);

                timeSheet = new TimeSheet(user!, request.Year, request.Week);

                _context.TimeSheets.Add(timeSheet);

                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }

            var periods = await _reportingPeriodRepository.GetReportingPeriodForTimeSheet(timeSheet, cancellationToken);

            return timeSheet.ToDto(periods);
        }
    }
}