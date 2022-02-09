
using MediatR;

using Microsoft.EntityFrameworkCore;

using TimeReport.Application.Common.Interfaces;

namespace TimeReport.Application.Projects.Queries;

public class GetProjectQuery : IRequest<ProjectDto?>
{
    public GetProjectQuery(string projectId)
    {
        ProjectId = projectId;
    }

    public string ProjectId { get; }

    public class GetProjectQueryHandler : IRequestHandler<GetProjectQuery, ProjectDto?>
    {
        private readonly ITimeReportContext _context;

        public GetProjectQueryHandler(ITimeReportContext context)
        {
            _context = context;
        }

        public async Task<ProjectDto?> Handle(GetProjectQuery request, CancellationToken cancellationToken)
        {
            var project = await _context.Projects
               .AsNoTracking()
               .AsSplitQuery()
               .FirstOrDefaultAsync(x => x.Id == request.ProjectId);

            if (project is null)
            {
                return null;
            }

            return new ProjectDto(project.Id, project.Name, project.Description);
        }
    }
}