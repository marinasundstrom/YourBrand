
using MediatR;

using Microsoft.EntityFrameworkCore;

using Skynet.TimeReport.Application.Common.Interfaces;
using Skynet.TimeReport.Application.Projects;
using Skynet.TimeReport.Application.Users;
using Skynet.TimeReport.Domain.Entities;

namespace Skynet.TimeReport.Application.TimeSheets.Queries;

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

            var monthInfo = await _context.MonthEntryGroups
                .Where(x => x.User.Id == timeSheet.User.Id)
                .Where(x => x.Month == timeSheet.From.Month || x.Month == timeSheet.To.Month)
                .ToArrayAsync(cancellationToken);

            return timeSheet.ToDto(monthInfo);
        }
    }
}