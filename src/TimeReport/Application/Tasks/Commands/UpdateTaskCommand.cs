
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.TimeReport.Application.Common.Interfaces;

namespace YourBrand.TimeReport.Application.Tasks.Commands;

public record UpdateTaskCommand(string OrganizationId, string TaskId, string Name, string TaskTypeId, string? Description, decimal? HourlyRate) : IRequest<TaskDto>
{
    public class UpdateTaskCommandHandler(ITimeReportContext context) : IRequestHandler<UpdateTaskCommand, TaskDto>
    {
        public async Task<TaskDto> Handle(UpdateTaskCommand request, CancellationToken cancellationToken)
        {
            var task = await context.Tasks
                .InOrganization(request.OrganizationId)
                .Include(x => x.Project)
                .AsSplitQuery()
                .FirstOrDefaultAsync(x => x.Id == request.TaskId, cancellationToken);

            if (task is null)
            {
                throw new Exception();
            }

            task.Name = request.Name;
            task.TaskType = await context.TaskTypes.FirstAsync(at => at.Id == request.TaskTypeId);
            task.Description = request.Description;
            task.HourlyRate = request.HourlyRate;

            await context.SaveChangesAsync(cancellationToken);

            task = await context.Tasks
               .InOrganization(request.OrganizationId)
               .Include(x => x.TaskType)
               .Include(x => x.Project)
               .ThenInclude(x => x.Organization)
               .AsNoTracking()
               .AsSplitQuery()
               .FirstOrDefaultAsync(x => x.Id == request.TaskId, cancellationToken);

            return task.ToDto();
        }
    }
}