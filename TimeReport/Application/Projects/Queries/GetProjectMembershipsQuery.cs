
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.TimeReport.Application.Common.Interfaces;
using YourBrand.TimeReport.Application.Common.Models;
using YourBrand.TimeReport.Application.Users;
using YourBrand.TimeReport.Domain.Exceptions;

namespace YourBrand.TimeReport.Application.Projects.Queries;

public record GetProjectMembershipsQuery(string ProjectId, int Page = 0, int PageSize = 10, string? SortBy = null, Application.Common.Models.SortDirection? SortDirection = null) : IRequest<ItemsResult<ProjectMembershipDto>>
{
    public class GetProjectMembershipsQueryHandler : IRequestHandler<GetProjectMembershipsQuery, ItemsResult<ProjectMembershipDto>>
    {
        private readonly ITimeReportContext _context;

        public GetProjectMembershipsQueryHandler(ITimeReportContext context)
        {
            _context = context;
        }

        public async Task<ItemsResult<ProjectMembershipDto>> Handle(GetProjectMembershipsQuery request, CancellationToken cancellationToken)
        {
            var project = await _context.Projects
                .OrderBy(p => p.Created)
                .AsSplitQuery()
                .FirstOrDefaultAsync(x => x.Id == request.ProjectId);

            if (project is null)
            {
                throw new ProjectNotFoundException(request.ProjectId);
            }

            var query = _context.ProjectMemberships
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