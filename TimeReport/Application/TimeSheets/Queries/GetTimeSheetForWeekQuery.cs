using System.Globalization;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.TimeReport.Application.Common.Interfaces;
using YourBrand.TimeReport.Application.Projects;
using YourBrand.TimeReport.Application.Users;
using YourBrand.TimeReport.Application.Users.Commands;
using YourBrand.TimeReport.Domain.Entities;

namespace YourBrand.TimeReport.Application.TimeSheets.Queries;

public record GetTimeSheetForWeekQuery(int Year, int Week, string? UserId) : IRequest<TimeSheetDto?>
{
    public class GetTimeSheetForWeekQueryHandler : IRequestHandler<GetTimeSheetForWeekQuery, TimeSheetDto?>
    {
        private readonly ITimeReportContext _context;
        private readonly ICurrentUserService _currentUserService;

        public GetTimeSheetForWeekQueryHandler(ITimeReportContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<TimeSheetDto?> Handle(GetTimeSheetForWeekQuery request, CancellationToken cancellationToken)
        {
            var query = _context.TimeSheets
                .Include(x => x.User)
                .Include(x => x.Activities)
                .ThenInclude(x => x.Entries)
                .ThenInclude(x => x.MonthGroup)
                .Include(x => x.Activities)
                .ThenInclude(x => x.Activity)
                .ThenInclude(x => x.Project)
                .ThenInclude(x => x.Organization)
                .Include(x => x.Activities)
                .ThenInclude(x => x.Project)
                .ThenInclude(x => x.Organization)
                .Include(x => x.Activities)
                .AsSplitQuery();

            string? userId = request.UserId ?? _currentUserService.UserId;

            query = query.Where(x => x.UserId == userId);

            var timeSheet = await query.FirstOrDefaultAsync(x => x.Year == request.Year && x.Week == request.Week, cancellationToken);

            if (timeSheet is null)
            {
                User? user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
                
                userId = user?.Id;

                var startDate = ISOWeek.ToDateTime(request.Year, request.Week, DayOfWeek.Monday);

                timeSheet = new TimeSheet(user!, request.Year, request.Week);

                _context.TimeSheets.Add(timeSheet);

                await _context.SaveChangesAsync(cancellationToken);
            }

            var monthInfo = await _context.TimeSheetMonths
                .Where(x => x.UserId == timeSheet.UserId)
                .Where(x => x.Month == timeSheet.From.Month || x.Month == timeSheet.To.Month)
                .ToArrayAsync(cancellationToken);

            return timeSheet.ToDto(monthInfo);
        }
    }
}