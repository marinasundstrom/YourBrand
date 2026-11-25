
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.TimeReport.Features.Common.Interfaces;

namespace YourBrand.TimeReport.Features.Tasks.Commands;

public record DeleteTaskCommand(string OrganizationId, string TaskId) : IRequest
{
    public class DeleteTaskCommandHandler(ITimeReportContext context) : IRequestHandler<DeleteTaskCommand>
    {
        public async Task Handle(DeleteTaskCommand request, CancellationToken cancellationToken)
        {
            var task = await context.Tasks
                .InOrganization(request.OrganizationId)
                .AsSplitQuery()
                .FirstOrDefaultAsync(x => x.Id == request.TaskId, cancellationToken);

            if (task is null)
            {
                throw new Exception();
            }

            context.Tasks.Remove(task);

            await context.SaveChangesAsync(cancellationToken);

        }
    }
}