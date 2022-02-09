
using MediatR;

using Microsoft.EntityFrameworkCore;

using TimeReport.Application.Common.Interfaces;
using TimeReport.Domain.Exceptions;

namespace TimeReport.Application.Projects.Commands;

public class UpdateProjectCommand : IRequest<ProjectDto>
{
    public UpdateProjectCommand(string projectId, string name, string? description)
    {
        ProjectId = projectId;
        Name = name;
        Description = description;
    }

    public string ProjectId { get; }

    public string Name { get; }

    public string? Description { get; }

    public class UpdateProjectCommandHandler : IRequestHandler<UpdateProjectCommand, ProjectDto>
    {
        private readonly ITimeReportContext _context;

        public UpdateProjectCommandHandler(ITimeReportContext context)
        {
            _context = context;
        }

        public async Task<ProjectDto> Handle(UpdateProjectCommand request, CancellationToken cancellationToken)
        {
            var project = await _context.Projects
                .AsSplitQuery()
                .FirstOrDefaultAsync(x => x.Id == request.ProjectId, cancellationToken);

            if (project is null)
            {
                throw new ProjectNotFoundException(request.ProjectId);
            }

            project.Name = request.Name;
            project.Description = request.Description;

            await _context.SaveChangesAsync(cancellationToken);

            return new ProjectDto(project.Id, project.Name, project.Description);
        }
    }
}