
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.TimeReport.Application.Common.Interfaces;
using YourBrand.TimeReport.Application.Common.Models;
using YourBrand.TimeReport.Application.Projects;

namespace YourBrand.TimeReport.Application.Users.Queries;

public record GetUserProjectMembershipsQuery(string UserId, int Page = 0, int PageSize = 10, string? SortBy = null, Application.Common.Models.SortDirection? SortDirection = null) : IRequest<ItemsResult<ProjectMembershipDto>>
{
    public class GetUserProjectMembershipsQueryHandler(ITimeReportContext context) : IRequestHandler<GetUserProjectMembershipsQuery, ItemsResult<ProjectMembershipDto>>
    {
        public async Task<ItemsResult<ProjectMembershipDto>> Handle(GetUserProjectMembershipsQuery request, CancellationToken cancellationToken)
        {
            var query = context.ProjectMemberships
                    .OrderBy(p => p.Created)
                    .Where(x => x.User.Id == request.UserId);

            var totalItems = await query.CountAsync(cancellationToken);

            if (request.SortBy is not null)
            {
                query = query.OrderBy(request.SortBy, request.SortDirection == Application.Common.Models.SortDirection.Desc ? TimeReport.Application.SortDirection.Descending : TimeReport.Application.SortDirection.Ascending);
            }

            var projectMemberships = await query
                .Include(m => m.Project)
                .Include(m => m.User)
                .Skip(request.PageSize * request.Page)
                .Take(request.PageSize)
                .AsSplitQuery()
                .ToArrayAsync(cancellationToken);

            var dtos = projectMemberships
                .DistinctBy(x => x.Project) // Temp
                .Select(m => m.ToDto());

            return new ItemsResult<ProjectMembershipDto>(dtos, totalItems);
        }
    }
}