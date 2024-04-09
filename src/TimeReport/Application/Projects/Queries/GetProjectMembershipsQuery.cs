
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.TimeReport.Application.Common.Interfaces;
using YourBrand.TimeReport.Application.Common.Models;
using YourBrand.TimeReport.Domain.Exceptions;

namespace YourBrand.TimeReport.Application.Projects.Queries;

public record GetProjectMembershipsQuery(string ProjectId, int Page = 0, int PageSize = 10, string? SortBy = null, Application.Common.Models.SortDirection? SortDirection = null) : IRequest<ItemsResult<ProjectMembershipDto>>
{
    public class GetProjectMembershipsQueryHandler(ITimeReportContext context) : IRequestHandler<GetProjectMembershipsQuery, ItemsResult<ProjectMembershipDto>>
    {
        public async Task<ItemsResult<ProjectMembershipDto>> Handle(GetProjectMembershipsQuery request, CancellationToken cancellationToken)
        {
            var project = await context.Projects
                .OrderBy(p => p.Created)
                .AsSplitQuery()
                .FirstOrDefaultAsync(x => x.Id == request.ProjectId);

            if (project is null)
            {
                throw new ProjectNotFoundException(request.ProjectId);
            }

            var query = context.ProjectMemberships
                    .OrderBy(p => p.Created)
                    .Where(m => m.Project.Id == project.Id);

            var totalItems = await query.CountAsync();

            if (request.SortBy is not null)
            {
                query = query.OrderBy(request.SortBy, request.SortDirection == TimeReport.Application.Common.Models.SortDirection.Desc ? TimeReport.Application.SortDirection.Descending : TimeReport.Application.SortDirection.Ascending);
            }

            var memberships = await query
                    .Include(m => m.User)
                    .Include(x => x.Project)
                    .ThenInclude(x => x.Organization)
                    .Skip(request.PageSize * request.Page)
                    .Take(request.PageSize)
                    .ToArrayAsync();

            var dtos = memberships
                .Select(m => m.ToDto());

            return new ItemsResult<ProjectMembershipDto>(dtos, totalItems);
        }
    }
}