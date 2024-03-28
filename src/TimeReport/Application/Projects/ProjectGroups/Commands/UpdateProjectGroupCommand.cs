
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.TimeReport.Application.Common.Interfaces;
using YourBrand.TimeReport.Application.Projects;

namespace YourBrand.TimeReport.Application.Projects.ProjectGroups.Commands;

public record UpdateProjectGroupCommand(string ExpenseId, string Name, string? Description) : IRequest<ProjectGroupDto>
{
    public class UpdateExpenseCommandHandler : IRequestHandler<UpdateProjectGroupCommand, ProjectGroupDto>
    {
        private readonly ITimeReportContext _context;

        public UpdateExpenseCommandHandler(ITimeReportContext context)
        {
            _context = context;
        }

        public async Task<ProjectGroupDto> Handle(UpdateProjectGroupCommand request, CancellationToken cancellationToken)
        {
            var projectGroup = await _context.ProjectGroups
                .Include(x => x.Project)
                .AsSplitQuery()
                .FirstOrDefaultAsync(x => x.Id == request.ExpenseId, cancellationToken);

            if (projectGroup is null)
            {
                throw new Exception();
            }

            projectGroup.Name = request.Name;
            projectGroup.Description = request.Description;

            await _context.SaveChangesAsync(cancellationToken);

            return projectGroup.ToDto();
        }
    }
}