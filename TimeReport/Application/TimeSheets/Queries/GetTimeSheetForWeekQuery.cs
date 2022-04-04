using System.Globalization;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.TimeReport.Application.Common.Interfaces;
using YourBrand.TimeReport.Application.Projects;
using YourBrand.TimeReport.Application.Users;
using YourBrand.TimeReport.Application.Users.Commands;
using YourBrand.TimeReport.Domain.Entities;

namespace YourBrand.TimeReport.Application.TimeSheets.Queries;

public class GetTimeSheetForWeekQuery : IRequest<TimeSheetDto?>
{
    public GetTimeSheetForWeekQuery(int year, int week, string? userId)
    {
        Year = year;
        Week = week;
        UserId = userId;
    }

    public int Year { get; }

    public int Week { get; }

    public string? UserId { get; }

    public class GetTimeSheetForWeekQueryHandler : IRequestHandler<GetTimeSheetForWeekQuery, TimeSheetDto?>
    {
        private readonly ITimeReportContext _context;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMediator _mediator;

        public GetTimeSheetForWeekQueryHandler(ITimeReportContext context, ICurrentUserService currentUserService, IMediator mediator)
        {
            _context = context;
            _currentUserService = currentUserService;
            _mediator = mediator;
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
                .Include(x => x.Activities)
                .ThenInclude(x => x.Project)
                .Include(x => x.Activities)
                .AsSplitQuery();

            string? userId = request.UserId ?? _currentUserService.UserId;

            query = query.Where(x => x.UserId == userId);

            var timeSheet = await query.FirstOrDefaultAsync(x => x.Year == request.Year && x.Week == request.Week, cancellationToken);

            if (timeSheet is null)
            {
                User? user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
                
                userId = user?.Id;

                if(user is null)
                {
                    var userDto = await _mediator.Send(new CreateUserCommand(
                        _currentUserService.UserId,
                        _currentUserService.FirstName,
                         _currentUserService.LastName,
                         string.Empty,
                         string.Empty,
                         _currentUserService.Email
                    ));

                    userId = userDto.Id;
                }

                var startDate = ISOWeek.ToDateTime(request.Year, request.Week, DayOfWeek.Monday);

                timeSheet = new TimeSheet()
                {
                    Id = Guid.NewGuid().ToString(),
                    Year = request.Year,
                    Week = request.Week,
                    From = startDate,
                    To = startDate.AddDays(6),
                    UserId = userId!
                };

                _context.TimeSheets.Add(timeSheet);

                await _context.SaveChangesAsync(cancellationToken);
            }

            var monthInfo = await _context.MonthEntryGroups
                .Where(x => x.UserId == timeSheet.UserId)
                .Where(x => x.Month == timeSheet.From.Month || x.Month == timeSheet.To.Month)
                .ToArrayAsync(cancellationToken);

            return timeSheet.ToDto(monthInfo);
        }
    }
}