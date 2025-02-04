
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.TimeReport.Application.Common.Interfaces;

namespace YourBrand.TimeReport.Application.Tasks.Queries;

public record GetTaskQuery(string OrganizationId, string TaskId) : IRequest<TaskDto>
{
    public class GetTaskQueryHandler(ITimeReportContext context) : IRequestHandler<GetTaskQuery, TaskDto>
    {
        public async Task<TaskDto> Handle(GetTaskQuery request, CancellationToken cancellationToken)
        {
            var task = await context.Tasks
               .InOrganization(request.OrganizationId)
               .Include(x => x.TaskType)
               .Include(x => x.Project)
               .ThenInclude(x => x.Organization)
               .AsNoTracking()
               .AsSplitQuery()
               .FirstOrDefaultAsync(x => x.Id == request.TaskId, cancellationToken);

            if (task is null)
            {
                throw new Exception();
            }

            return task.ToDto();
        }
    }
}