
using MediatR;

using Microsoft.EntityFrameworkCore;

using TimeReport.Application.Common.Interfaces;
using TimeReport.Application.Common.Models;
using TimeReport.Application.Users;
using TimeReport.Domain.Exceptions;

namespace TimeReport.Application.Projects.Queries;

public class GetProjectMembershipsQuery : IRequest<ItemsResult<ProjectMembershipDto>>
{
    public GetProjectMembershipsQuery(string projectId, int page = 0, int pageSize = 10, string? sortBy = null, Application.Common.Models.SortDirection? sortDirection = null)
    {
        ProjectId = projectId;
        Page = page;
        PageSize = pageSize;
        SortBy = sortBy;
        SortDirection = sortDirection;
    }

    public string ProjectId { get; }

    public int Page { get; }

    public int PageSize { get; }

    public string? SearchString { get; }

    public string? SortBy { get; }

    public Application.Common.Models.SortDirection? SortDirection { get; }

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
                    .Skip(request.PageSize * request.Page)
                    .Take(request.PageSize)
                    .ToArrayAsync();

            var dtos = memberships
                .Select(m => new ProjectMembershipDto(m.Id, new ProjectDto(m.Project.Id, m.Project.Name, m.Project.Description),
                new UserDto(m.User.Id, m.User.FirstName, m.User.LastName, m.User.DisplayName, m.User.SSN, m.User.Email, m.User.Created, m.User.Deleted),
                m.From, m.Thru));

            return new ItemsResult<ProjectMembershipDto>(dtos, totalItems);
        }
    }
}