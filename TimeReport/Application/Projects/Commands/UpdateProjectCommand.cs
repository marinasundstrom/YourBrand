
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.TimeReport.Application.Common.Interfaces;
using YourBrand.TimeReport.Domain.Exceptions;

namespace YourBrand.TimeReport.Application.Projects.Commands;

public record UpdateProjectCommand(string ProjectId, string Name, string? Description) : IRequest<ProjectDto>
{
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