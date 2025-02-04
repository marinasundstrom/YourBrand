
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.TimeReport.Application.Common.Interfaces;
using YourBrand.TimeReport.Domain.Entities;

namespace YourBrand.TimeReport.Application.Tasks.TaskTypes.Commands;

public record CreateTaskTypeCommand(string OrganizationId, string Name, string? Description, string? ProjectId, bool ExcludeHours) : IRequest<TaskTypeDto>
{
    public class CreateTaskCommandHandler(ITimeReportContext context) : IRequestHandler<CreateTaskTypeCommand, TaskTypeDto>
    {
        public async Task<TaskTypeDto> Handle(CreateTaskTypeCommand request, CancellationToken cancellationToken)
        {
            Project? project = null;

            Organization organization = await context.Organizations
                    .AsSplitQuery()
                    .FirstAsync(x => x.Id == request.OrganizationId, cancellationToken);

            if (request.ProjectId is not null)
            {
                project = await context.Projects
                        .InOrganization(request.OrganizationId)
                        .AsSplitQuery()
                        .FirstAsync(x => x.Organization.Id == request.OrganizationId && x.Id == request.ProjectId, cancellationToken);
            }

            var taskType = new TaskType(request.Name, request.Description)
            {
                Organization = organization,
                Project = project,
                ExcludeHours = request.ExcludeHours
            };

            context.TaskTypes.Add(taskType);

            await context.SaveChangesAsync(cancellationToken);

            return taskType.ToDto();
        }
    }
}