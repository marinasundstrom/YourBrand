
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.TimeReport.Application.Common.Interfaces;

namespace YourBrand.TimeReport.Application.Tasks.TaskCharges.Queries;

public record GetTaskChargeQuery(string OrganizationId, string ChargeId) : IRequest<TaskChargeDto>
{
    public class GetTaskQueryHandler(ITimeReportContext context) : IRequestHandler<GetTaskChargeQuery, TaskChargeDto>
    {
        public async Task<TaskChargeDto> Handle(GetTaskChargeQuery request, CancellationToken cancellationToken)
        {
            var taskCharge = await context.TaskCharges
                .InOrganization(request.OrganizationId)
               .Include(x => x.Task)
               .AsNoTracking()
               .AsSplitQuery()
               .FirstOrDefaultAsync(x => x.Id == request.ChargeId, cancellationToken);

            if (taskCharge is null)
            {
                throw new Exception();
            }

            return taskCharge.ToDto();
        }
    }
}