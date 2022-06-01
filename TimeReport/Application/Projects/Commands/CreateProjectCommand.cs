
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.TimeReport.Application.Common.Interfaces;
using YourBrand.TimeReport.Domain.Entities;

namespace YourBrand.TimeReport.Application.Projects.Commands;

public record CreateProjectCommand(string Name, string? Description, string OrganizationId) : IRequest<ProjectDto>
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
            var project = new Project(request.Name, request.Description)
            {
                Organization = await _context.Organizations.FirstAsync() //.FirstAsync(o => o.Id == request.OrganizationId)
            };

            _context.Projects.Add(project);

            await _context.SaveChangesAsync(cancellationToken);

            return project.ToDto();
        }
    }
}