
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.TimeReport.Features.Common.Interfaces;
using YourBrand.TimeReport.Features.Common.Models;

namespace YourBrand.TimeReport.Features.Projects.Queries;

public record GetProjectMembershipsQuery(string OrganizationId, string ProjectId, int Page = 0, int PageSize = 10, string? SortBy = null, TimeReport.Features.Common.Models.SortDirection? SortDirection = null) : IRequest<Result<ItemsResult<ProjectMembershipDto>>>
{
    public class GetProjectMembershipsQueryHandler(ITimeReportContext context) : IRequestHandler<GetProjectMembershipsQuery, Result<ItemsResult<ProjectMembershipDto>>>
    {
        public async Task<Result<ItemsResult<ProjectMembershipDto>>> Handle(GetProjectMembershipsQuery request, CancellationToken cancellationToken)
        {
            var project = await context.Projects
                .InOrganization(request.OrganizationId)
                .OrderBy(p => p.Created)
                .AsSplitQuery()
                .FirstOrDefaultAsync(x => x.Id == request.ProjectId);

            if (project is null)
            {
                return new ProjectNotFound(request.ProjectId);
            }

            var query = context.ProjectMemberships
                    .OrderBy(p => p.Created)
                    .Where(m => m.Project.Id == project.Id);

            var totalItems = await query.CountAsync();

            if (request.SortBy is not null)
            {
                query = query.OrderBy(request.SortBy, request.SortDirection == TimeReport.Features.Common.Models.SortDirection.Desc ? TimeReport.Features.SortDirection.Descending : TimeReport.Features.SortDirection.Ascending);
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