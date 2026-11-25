
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.TimeReport.Application.Common.Interfaces;
using YourBrand.TimeReport.Domain.Entities;

namespace YourBrand.TimeReport.Application.Tasks.TaskCharges.Commands;

public record CreateTaskChargeCommand(string OrganizationId, string? Description, string? ProjectId, string? TaskId,
    ChargeType ChargeType, ChargeBasis Basis, decimal Amount) : IRequest<TaskChargeDto>
{
    public class CreateTaskCommandHandler(ITimeReportContext context) : IRequestHandler<CreateTaskChargeCommand, TaskChargeDto>
    {
        public async Task<TaskChargeDto> Handle(CreateTaskChargeCommand request, CancellationToken cancellationToken)
        {
            YourBrand.TimeReport.Domain.Entities.Project? project = null;
            YourBrand.TimeReport.Domain.Entities.Task? task = null;

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

            if (request.TaskId is not null)
            {
                task = await context.Tasks
                        .InOrganization(request.OrganizationId)
                        .AsSplitQuery()
                        .FirstAsync(x => x.OrganizationId == request.OrganizationId && x.Id == request.TaskId, cancellationToken);
            }

            var taskCharge = new TaskCharge(request.Description)
            {
                Project = project ?? task.Project,
                Task = task,
                Type = request.ChargeType,
                Basis = request.Basis,
                Amount = request.Amount,
            };

            context.TaskCharges.Add(taskCharge);

            await context.SaveChangesAsync(cancellationToken);

            return taskCharge.ToDto();
        }
    }
}