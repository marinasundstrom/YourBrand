
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.TimeReport.Features.Common.Interfaces;

namespace YourBrand.TimeReport.Features.Tasks.TaskTypes.Commands;

public record DeleteTaskTypeCommand(string OrganizationId, string TypeId) : IRequest
{
    public class DeleteTaskCommandHandler(ITimeReportContext context) : IRequestHandler<DeleteTaskTypeCommand>
    {
        public async Task Handle(DeleteTaskTypeCommand request, CancellationToken cancellationToken)
        {
            var taskType = await context.TaskTypes
                .InOrganization(request.OrganizationId)
                .AsSplitQuery()
                .FirstOrDefaultAsync(x => x.Id == request.TypeId, cancellationToken);

            if (taskType is null)
            {
                throw new Exception();
            }

            context.TaskTypes.Remove(taskType);

            await context.SaveChangesAsync(cancellationToken);

        }
    }
}