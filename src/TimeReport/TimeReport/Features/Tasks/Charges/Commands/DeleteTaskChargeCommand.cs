
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.TimeReport.Features.Common.Interfaces;

namespace YourBrand.TimeReport.Features.Tasks.TaskCharges.Commands;

public record DeleteTaskChargeCommand(string OrganizationId, string ChargeId) : IRequest
{
    public class DeleteTaskCommandHandler(ITimeReportContext context) : IRequestHandler<DeleteTaskChargeCommand>
    {
        public async Task Handle(DeleteTaskChargeCommand request, CancellationToken cancellationToken)
        {
            var taskCharge = await context.TaskCharges
                .InOrganization(request.OrganizationId)
                .AsSplitQuery()
                .FirstOrDefaultAsync(x => x.Id == request.ChargeId, cancellationToken);

            if (taskCharge is null)
            {
                throw new Exception();
            }

            context.TaskCharges.Remove(taskCharge);

            await context.SaveChangesAsync(cancellationToken);

        }
    }
}