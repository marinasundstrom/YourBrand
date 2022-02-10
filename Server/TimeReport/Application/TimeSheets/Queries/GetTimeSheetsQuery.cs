
using MediatR;

using Microsoft.EntityFrameworkCore;

using Skynet.TimeReport.Application.Common.Interfaces;
using Skynet.TimeReport.Application.Common.Models;
using Skynet.TimeReport.Application.Projects;
using Skynet.TimeReport.Application.Users;
using Skynet.TimeReport.Domain.Entities;

namespace Skynet.TimeReport.Application.TimeSheets.Queries;

public class GetTimeSheetsQuery : IRequest<ItemsResult<TimeSheetDto>>
{
    public GetTimeSheetsQuery(int page = 0, int pageSize = 10, string? projectId = null, string? searchString = null, string? sortBy = null, Common.Models.SortDirection? sortDirection = null)
    {
        Page = page;
        PageSize = pageSize;
        ProjectId = projectId;
        SearchString = searchString;
        SortBy = sortBy;
        SortDirection = sortDirection;
    }

    public int Page { get; }

    public int PageSize { get; }

    public string? ProjectId { get; }

    public string? SearchString { get; }

    public string? SortBy { get; }

    public Application.Common.Models.SortDirection? SortDirection { get; }

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

            if (request.SearchString is not null)
            {
                query = query.Where(timeSheet => timeSheet.Id.ToLower().Contains(request.SearchString.ToLower()));
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
                            .Where(x => x.User.Id == timeSheet.User.Id)
                            .Where(x => x.Month == timeSheet.From.Month || x.Month == timeSheet.To.Month);

                    return timeSheet.ToDto(m);
                }),
                totalItems);
        }
    }
}