
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.TimeReport.Application.Common.Interfaces;

namespace YourBrand.TimeReport.Application.Projects.Queries;

public record GetProjectQuery(string ProjectId) : IRequest<ProjectDto?>
{
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