
using MediatR;

using YourBrand.TimeReport.Application.Common.Interfaces;
using YourBrand.TimeReport.Domain.Entities;

namespace YourBrand.TimeReport.Application.Projects.Commands;

public record CreateProjectCommand(string Name, string? Description) : IRequest<ProjectDto>
{
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