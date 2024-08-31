
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.TimeReport.Application.Common.Interfaces;
using YourBrand.TimeReport.Application.Common.Models;

namespace YourBrand.TimeReport.Application.Users.Absence.Queries;

public record GetAbsencesQuery(string OrganizationId, int Page = 0, int PageSize = 10, string? ProjectId = null, string? SearchString = null, string? SortBy = null, Application.Common.Models.SortDirection? SortDirection = null) : IRequest<ItemsResult<AbsenceDto>>
{
    public class GetAbsencesQueryHandler(ITimeReportContext context) : IRequestHandler<GetAbsencesQuery, ItemsResult<AbsenceDto>>
    {
        public async Task<ItemsResult<AbsenceDto>> Handle(GetAbsencesQuery request, CancellationToken cancellationToken)
        {
            var query = context.Absence
                .Include(x => x.Project)
                .OrderBy(p => p.Created)
                .AsNoTracking()
                .AsSplitQuery();

            if (request.ProjectId is not null)
            {
                query = query.Where(absence => absence.Project.Id == request.ProjectId);
            }

            if (request.SearchString is not null)
            {
                query = query.Where(absence => absence.Note.ToLower().Contains(request.SearchString.ToLower()));
            }

            var totalItems = await query.CountAsync(cancellationToken);

            if (request.SortBy is not null)
            {
                query = query.OrderBy(request.SortBy, request.SortDirection == Application.Common.Models.SortDirection.Desc ? TimeReport.Application.SortDirection.Descending : TimeReport.Application.SortDirection.Ascending);
            }

            var absences = await query
                .Skip(request.PageSize * request.Page)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            var dtos = absences.Select(absence => absence.ToDto());

            return new ItemsResult<AbsenceDto>(dtos, totalItems);
        }
    }
}