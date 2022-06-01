
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.TimeReport.Application.Common.Interfaces;
using YourBrand.TimeReport.Application.Projects;
using YourBrand.TimeReport.Application.Users;
using YourBrand.TimeReport.Domain.Entities;

namespace YourBrand.TimeReport.Application.TimeSheets.Queries;

public record GetTimeSheetQuery(string TimeSheetId) : IRequest<TimeSheetDto?>
{
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
                .ThenInclude(x => x.Organization)
                .Include(x => x.Activities)
                .ThenInclude(x => x.Activity)
                .ThenInclude(x => x.Project)
                .ThenInclude(x => x.Organization)
                .AsNoTracking()
                .AsSplitQuery()
                .FirstOrDefaultAsync(x => x.Id == request.TimeSheetId, cancellationToken);

            if (timeSheet is null)
            {
                return null;
            }

            var monthInfo = await _context.TimeSheetMonths
                .Where(x => x.UserId == timeSheet.UserId)
                .Where(x => x.Month == timeSheet.From.Month || x.Month == timeSheet.To.Month)
                .ToArrayAsync(cancellationToken);

            return timeSheet.ToDto(monthInfo);
        }
    }
}