
using MediatR;

using YourBrand.TimeReport.Application.Common.Interfaces;
using YourBrand.TimeReport.Domain.Entities;

namespace YourBrand.TimeReport.Application.Projects.ProjectGroups.Commands;

public record CreateProjectGroupCommand(string Name, string? Description) : IRequest<ProjectGroupDto>
{
    public class CreateExpenseCommandHandler : IRequestHandler<CreateProjectGroupCommand, ProjectGroupDto>
    {
        private readonly ITimeReportContext _context;

        public CreateExpenseCommandHandler(ITimeReportContext context)
        {
            _context = context;
        }

        public async Task<ProjectGroupDto> Handle(CreateProjectGroupCommand request, CancellationToken cancellationToken)
        {
            /*
            var project = await _context.Projects
               .AsSplitQuery()
               .FirstOrDefaultAsync(x => x.Id == request.ProjectId, cancellationToken);

            if (project is null)
            {
                throw new Exception();
            }
            */

            var projectGroup = new ProjectGroup
            {
                Id = Guid.NewGuid().ToString(),
                Name = request.Name,
                Description = request.Description,
                //Project = project
            };

            _context.ProjectGroups.Add(projectGroup);

            await _context.SaveChangesAsync(cancellationToken);

            return projectGroup.ToDto();
        }
    }
}