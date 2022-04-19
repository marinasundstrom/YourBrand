
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.TimeReport.Application.Common.Interfaces;
using YourBrand.TimeReport.Application.Common.Models;
using YourBrand.TimeReport.Application.Projects;
using YourBrand.TimeReport.Application.Users;
using YourBrand.TimeReport.Domain.Entities;

namespace YourBrand.TimeReport.Application.TimeSheets.Queries;

public record GetTimeSheetsQuery(int Page = 0, int PageSize = 10, string? ProjectId = null, string? UserId = null, string? SearchString = null, string? SortBy = null, Common.Models.SortDirection? SortDirection = null) : IRequest<ItemsResult<TimeSheetDto>>
{
    public class GetTimeSheetsQueryHandler : IRequestHandler<GetTimeSheetsQuery, ItemsResult<TimeSheetDto>>
    {
        private readonly ITimeReportContext _context;

        public GetTimeSheetsQueryHandler(ITimeReportContext context)
        {
            _context = context;
        }

        public async Task<ItemsResult<TimeSheetDto>> Handle(GetTimeSheetsQuery request, CancellationToken cancellationToken)
        {
            var query = _context.TimeSheets
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
                       .OrderByDescending(x => x.Year)
                       .ThenByDescending(x => x.Week)
                       .AsNoTracking()
                       .AsSplitQuery();

            if (request.ProjectId is not null)
            {
                query = query.Where(timeSheet => timeSheet.Activities.Any(x => x.Project.Id == request.ProjectId));
            }

             if (request.UserId is not null)
            {
                query = query.Where(timeSheet => timeSheet.UserId == request.UserId);
            }

            if (request.SearchString is not null)
            {
                query = query.Where(timeSheet =>
                    timeSheet.Id.ToLower().Contains(request.SearchString.ToLower())
                    || timeSheet.Week.ToString().Contains(request.SearchString.ToLower())
                    || timeSheet.User.FirstName.ToLower().Contains(request.SearchString.ToLower())
                    || timeSheet.User.LastName.ToLower().Contains(request.SearchString.ToLower())
                    || timeSheet.User.DisplayName.ToLower().Contains(request.SearchString.ToLower()));
            }

            var totalItems = await query.CountAsync(cancellationToken);

            if (request.SortBy is not null)
            {
                query = query.OrderBy(request.SortBy, request.SortDirection == TimeReport.Application.Common.Models.SortDirection.Desc ? TimeReport.Application.SortDirection.Descending : TimeReport.Application.SortDirection.Ascending);
            }

            var timeSheets = await query
                .Skip(request.PageSize * request.Page)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            var monthInfo = await _context.MonthEntryGroups
                .Where(x => x.Status == EntryStatus.Locked)
                .ToArrayAsync(cancellationToken);

            return new ItemsResult<TimeSheetDto>(
                timeSheets.Select(timeSheet =>
                {
                    var m = monthInfo
                            .Where(x => x.UserId == timeSheet.UserId)
                            .Where(x => x.Month == timeSheet.From.Month || x.Month == timeSheet.To.Month);

                    return timeSheet.ToDto(m);
                }),
                totalItems);
        }
    }
}