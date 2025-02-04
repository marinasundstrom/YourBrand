
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.TimeReport.Application.Common.Interfaces;
using YourBrand.TimeReport.Domain.Entities;

namespace YourBrand.TimeReport.Application.Tasks.TaskTypes.Commands;

public record UpdateTaskTypeCommand(string OrganizationId, string TaskId, string Name, string? Description, string? ProjectId, bool ExcludeHours) : IRequest<TaskTypeDto>
{
    public class UpdateTaskCommandHandler(ITimeReportContext context) : IRequestHandler<UpdateTaskTypeCommand, TaskTypeDto>
    {
        public async Task<TaskTypeDto> Handle(UpdateTaskTypeCommand request, CancellationToken cancellationToken)
        {
            var taskType = await context.TaskTypes
                .InOrganization(request.OrganizationId)
                .Include(x => x.Project)
                .AsSplitQuery()
                .FirstOrDefaultAsync(x => x.Id == request.TaskId, cancellationToken);

            if (taskType is null)
            {
                throw new Exception();
            }

            Project? project = null;

            Organization organization = await context.Organizations
                    .AsSplitQuery()
                    .FirstAsync(x => x.Id == request.OrganizationId, cancellationToken);

            if (request.ProjectId is not null)
            {
                project = await context.Projects
                        .AsSplitQuery()
                        .FirstAsync(x => x.Organization.Id == request.OrganizationId && x.Id == request.ProjectId, cancellationToken);
            }

            taskType.Name = request.Name;
            taskType.Description = request.Description;
            taskType.Organization = organization;
            taskType.Project = project;
            taskType.ExcludeHours = request.ExcludeHours;

            await context.SaveChangesAsync(cancellationToken);

            return taskType.ToDto();
        }
    }
}