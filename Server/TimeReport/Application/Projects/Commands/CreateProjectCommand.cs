
using MediatR;

using TimeReport.Application.Common.Interfaces;
using TimeReport.Domain.Entities;

namespace TimeReport.Application.Projects.Commands;

public class CreateProjectCommand : IRequest<ProjectDto>
{
    public CreateProjectCommand(string name, string? description)
    {
        Name = name;
        Description = description;
    }

    public string Name { get; }

    public string? Description { get; }

    public class CreateProjectCommandHandler : IRequestHandler<CreateProjectCommand, ProjectDto>
    {
        private readonly ITimeReportContext _context;

        public CreateProjectCommandHandler(ITimeReportContext context)
        {
            _context = context;
        }

        public async Task<ProjectDto> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
        {
            var project = new Project
            {
                Id = Guid.NewGuid().ToString(),
                Name = request.Name,
                Description = request.Description
            };

            _context.Projects.Add(project);

            await _context.SaveChangesAsync(cancellationToken);

            return new ProjectDto(project.Id, project.Name, project.Description);
        }
    }
}