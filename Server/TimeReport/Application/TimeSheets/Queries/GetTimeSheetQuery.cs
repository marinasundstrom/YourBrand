
using MediatR;

using Microsoft.EntityFrameworkCore;

using TimeReport.Application.Common.Interfaces;
using TimeReport.Application.Projects;
using TimeReport.Application.Users;
using TimeReport.Domain.Entities;

namespace TimeReport.Application.TimeSheets.Queries;

public class GetTimeSheetQuery : IRequest<TimeSheetDto?>
{
    public GetTimeSheetQuery(string timeSheetId)
    {
        TimeSheetId = timeSheetId;
    }

    public string TimeSheetId { get; }

    public class GetTimeSheetQueryHandler : IRequestHandler<GetTimeSheetQuery, TimeSheetDto?>
    {
        private readonly ITimeReportContext _context;

        public GetTimeSheetQueryHandler(ITimeReportContext context)
        {
            _context = context;
        }

        public async Task<TimeSheetDto?> Handle(GetTimeSheetQuery request, CancellationToken cancellationToken)
        {
            var timeSheet = await _context.TimeSheets
                .Include(x => x.User)
                .Include(x => x.Activities)
                .ThenInclude(x => x.Entries)
                .ThenInclude(x => x.MonthGroup)
                .Include(x => x.Activities)
                .ThenInclude(x => x.Activity)
                .Include(x => x.Activities)
                .ThenInclude(x => x.Project)
                .Include(x => x.Activities)
                .ThenInclude(x => x.Activity)
                .ThenInclude(x => x.Project)
                .AsNoTracking()
                .AsSplitQuery()
                .FirstOrDefaultAsync(x => x.Id == request.TimeSheetId, cancellationToken);

            if (timeSheet is null)
            {
                return null;
            }

            var activities = timeSheet.Activities
                .OrderBy(e => e.Created)
                .Select(e => new TimeSheetActivityDto(e.Activity.Id, e.Activity.Name, e.Activity.Description, new ProjectDto(e.Project.Id, e.Project.Name, e.Project.Description),
                    e.Entries.OrderBy(e => e.Date).Select(e => new TimeSheetEntryDto(e.Id, e.Date.ToDateTime(TimeOnly.Parse("01:00")), e.Hours, e.Description, (EntryStatusDto)e.MonthGroup.Status))));

            var monthInfo = await _context.MonthEntryGroups
                .Where(x => x.User.Id == timeSheet.User.Id)
                .Where(x => x.Month == timeSheet.From.Month || x.Month == timeSheet.To.Month)
                .ToArrayAsync(cancellationToken);

            return new TimeSheetDto(timeSheet.Id, timeSheet.Year, timeSheet.Week, timeSheet.From, timeSheet.To, (TimeSheetStatusDto)timeSheet.Status, new UserDto(timeSheet.User.Id, timeSheet.User.FirstName, timeSheet.User.LastName, timeSheet.User.DisplayName, timeSheet.User.SSN, timeSheet.User.Email, timeSheet.User.Created, timeSheet.User.Deleted),
                activities, monthInfo.Select(x => new MonthInfoDto(x.Month, x.Status == EntryStatus.Locked)));
        }
    }
}