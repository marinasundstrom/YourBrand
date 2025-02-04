
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.TimeReport.Application.Common.Interfaces;

namespace YourBrand.TimeReport.Application.Tasks.TaskTypes.Commands;

public record DeleteTaskTypeCommand(string OrganizationId, string TaskId) : IRequest
{
    public class DeleteTaskCommandHandler(ITimeReportContext context) : IRequestHandler<DeleteTaskTypeCommand>
    {
        public async Task Handle(DeleteTaskTypeCommand request, CancellationToken cancellationToken)
        {
            var taskType = await context.TaskTypes
                .InOrganization(request.OrganizationId)
                .AsSplitQuery()
                .FirstOrDefaultAsync(x => x.Id == request.TaskId, cancellationToken);

            if (taskType is null)
            {
                throw new Exception();
            }

            context.TaskTypes.Remove(taskType);

            await context.SaveChangesAsync(cancellationToken);

        }
    }
}