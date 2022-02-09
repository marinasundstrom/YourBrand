
using MediatR;

using Microsoft.EntityFrameworkCore;

using TimeReport.Application.Common.Interfaces;
using TimeReport.Application.Projects;
using TimeReport.Application.Users;
using TimeReport.Domain.Entities;

namespace TimeReport.Application.TimeSheets.Queries;

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

        public GetTimeSheetForWeekQueryHandler(ITimeReportContext context)
        {
            _context = context;
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

            if (request.UserId is not null)
            {
                query = query.Where(x => x.User.Id == request.UserId);
            }

            var timeSheet = await query.FirstOrDefaultAsync(x => x.Year == request.Year && x.Week == request.Week, cancellationToken);

            if (timeSheet is null)
            {
                User? user = null;

                if (request.UserId is not null)
                {
                    user = await _context.Users.FirstAsync(x => x.Id == request.UserId, cancellationToken);
                }
                else
                {
                    user = await _context.Users.FirstOrDefaultAsync(cancellationToken);
                }

                var startDate = System.Globalization.ISOWeek.ToDateTime(request.Year, request.Week, DayOfWeek.Monday);

                timeSheet = new TimeSheet()
                {
                    Id = Guid.NewGuid().ToString(),
                    Year = request.Year,
                    Week = request.Week,
                    From = startDate,
                    To = startDate.AddDays(6),
                    User = user
                };

                _context.TimeSheets.Add(timeSheet);

                await _context.SaveChangesAsync(cancellationToken);
            }

            var activities = timeSheet.Activities
                .OrderBy(e => e.Created)
                .Select(e => new TimeSheetActivityDto(e.Activity.Id, e.Activity.Name, e.Activity.Description, new ProjectDto(e.Project.Id, e.Project.Name, e.Project.Description),
                    e.Entries.OrderBy(e => e.Date).Select(e => new TimeSheetEntryDto(e.Id, e.Date.ToDateTime(TimeOnly.Parse("01:00")), e.Hours, e.Description, (EntryStatusDto)e.MonthGroup.Status))))
                .ToArray();

            var monthInfo = await _context.MonthEntryGroups
                .Where(x => x.User.Id == timeSheet.User.Id)
                .Where(x => x.Month == timeSheet.From.Month || x.Month == timeSheet.To.Month)
                .ToArrayAsync(cancellationToken);

            return new TimeSheetDto(timeSheet.Id, timeSheet.Year, timeSheet.Week, timeSheet.From, timeSheet.To, (TimeSheetStatusDto)timeSheet.Status, new UserDto(timeSheet.User.Id, timeSheet.User.FirstName, timeSheet.User.LastName, timeSheet.User.DisplayName, timeSheet.User.SSN, timeSheet.User.Email, timeSheet.User.Created, timeSheet.User.Deleted),
                activities, monthInfo.Select(x => new MonthInfoDto(x.Month, x.Status == EntryStatus.Locked)));
        }
    }
}