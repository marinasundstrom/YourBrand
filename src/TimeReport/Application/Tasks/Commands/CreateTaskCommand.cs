
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.TimeReport.Application.Common.Interfaces;
using YourBrand.TimeReport.Domain.Entities;

namespace YourBrand.TimeReport.Application.Tasks.Commands;

public record CreateTaskCommand(string OrganizationId, string ProjectId, string Name, string TaskTypeId, string? Description, decimal? HourlyRate) : IRequest<TaskDto>
{
    public class CreateTaskCommandHandler(ITimeReportContext context) : IRequestHandler<CreateTaskCommand, TaskDto>
    {
        public async Task<TaskDto> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
        {
            var project = await context.Projects
               .InOrganization(request.OrganizationId)
               .AsSplitQuery()
               .Include(at => at.Organization)
               .ThenInclude(at => at.CreatedBy)
               .Include(at => at.Organization)
               .ThenInclude(at => at.LastModifiedBy)
               .Include(at => at.Organization)
               .ThenInclude(at => at.DeletedBy)
               .Include(at => at.CreatedBy)
               .Include(at => at.LastModifiedBy)
               .Include(at => at.DeletedBy)
               .FirstOrDefaultAsync(x => x.Id == request.ProjectId, cancellationToken);

            if (project is null)
            {
                throw new Exception();
            }

            var taskType = await context.TaskTypes
                    .AsSingleQuery()
                    .IncludeAll()
                    .FirstAsync(at => at.Id == request.TaskTypeId);

            var task = new Domain.Entities.Task(request.Name, taskType, request.Description)
            {
                OrganizationId = request.OrganizationId,
                Project = project,
                HourlyRate = request.HourlyRate
            };

            context.Tasks.Add(task);

            await context.SaveChangesAsync(cancellationToken);

            return task.ToDto();
        }
    }
}