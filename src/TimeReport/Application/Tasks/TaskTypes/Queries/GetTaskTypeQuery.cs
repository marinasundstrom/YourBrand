
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.TimeReport.Application.Common.Interfaces;

namespace YourBrand.TimeReport.Application.Tasks.TaskTypes.Queries;

public record GetTaskTypeQuery(string OrganizationId, string TaskId) : IRequest<TaskTypeDto>
{
    public class GetTaskQueryHandler(ITimeReportContext context) : IRequestHandler<GetTaskTypeQuery, TaskTypeDto>
    {
        public async Task<TaskTypeDto> Handle(GetTaskTypeQuery request, CancellationToken cancellationToken)
        {
            var taskType = await context.TaskTypes
                .InOrganization(request.OrganizationId)
               .Include(x => x.Project)
               .AsNoTracking()
               .AsSplitQuery()
               .FirstOrDefaultAsync(x => x.Id == request.TaskId, cancellationToken);

            if (taskType is null)
            {
                throw new Exception();
            }

            return taskType.ToDto();
        }
    }
}