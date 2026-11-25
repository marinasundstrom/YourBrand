
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.TimeReport.Application.Common.Interfaces;
using YourBrand.TimeReport.Domain.Entities;

namespace YourBrand.TimeReport.Application.Tasks.TaskCharges.Commands;

public record UpdateTaskChargeCommand(string OrganizationId, string ChargeId, string? Description, string? ProjectId, string? TaskId,
    ChargeType Type, ChargeBasis Basis, decimal Amount) : IRequest<TaskChargeDto>
{
    public class UpdateTaskCommandHandler(ITimeReportContext context) : IRequestHandler<UpdateTaskChargeCommand, TaskChargeDto>
    {
        public async Task<TaskChargeDto> Handle(UpdateTaskChargeCommand request, CancellationToken cancellationToken)
        {
            var taskCharge = await context.TaskCharges
                .InOrganization(request.OrganizationId)
                .Include(x => x.Project)
                .AsSplitQuery()
                .FirstOrDefaultAsync(x => x.Id == request.ChargeId, cancellationToken);

            if (taskCharge is null)
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

            taskCharge.Description = request.Description;
            taskCharge.OrganizationId = organization.Id;
            //taskCharge.Project = project;
            //taskCharge.Ticket = ticket;

            taskCharge.Type = request.Type;
            taskCharge.Basis = request.Basis;
            taskCharge.Amount = request.Amount;

            await context.SaveChangesAsync(cancellationToken);

            return taskCharge.ToDto();
        }
    }
}