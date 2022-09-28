
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.TimeReport.Application.Common.Interfaces;
using YourBrand.TimeReport.Application.Common.Models;
using YourBrand.TimeReport.Application.Projects;
using YourBrand.TimeReport.Application.Users;
using YourBrand.TimeReport.Domain;
using YourBrand.TimeReport.Domain.Entities;
using YourBrand.TimeReport.Domain.Repositories;

namespace YourBrand.TimeReport.Application.TimeSheets.Queries;

public record GetTimeSheetsQuery(int Page = 0, int PageSize = 10, string? ProjectId = null, string? UserId = null, string? SearchString = null, string? SortBy = null, Common.Models.SortDirection? SortDirection = null) : IRequest<ItemsResult<TimeSheetDto>>
{
    public class GetTimeSheetsQueryHandler : IRequestHandler<GetTimeSheetsQuery, ItemsResult<TimeSheetDto>>
    {
        private readonly ITimeSheetRepository _timeSheetRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITimeReportContext _context;

        public GetTimeSheetsQueryHandler(ITimeSheetRepository timeSheetRepository, IUnitOfWork unitOfWork, ITimeReportContext context)
        {
            _timeSheetRepository = timeSheetRepository;
            _unitOfWork = unitOfWork;
            _context = context;
        }

        public async Task<ItemsResult<TimeSheetDto>> Handle(GetTimeSheetsQuery request, CancellationToken cancellationToken)
        {
            var query = _timeSheetRepository.GetTimeSheets()
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

            var monthInfo = await _context.TimeSheetMonths
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