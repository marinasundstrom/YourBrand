
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.TimeReport.Application.Common.Interfaces;

namespace YourBrand.TimeReport.Application.Activities.ActivityTypes.Queries;

public record GetActivityTypeQuery(string OrganizationId, string ActivityId) : IRequest<ActivityTypeDto>
{
    public class GetActivityQueryHandler(ITimeReportContext context) : IRequestHandler<GetActivityTypeQuery, ActivityTypeDto>
    {
        public async Task<ActivityTypeDto> Handle(GetActivityTypeQuery request, CancellationToken cancellationToken)
        {
            var activityType = await context.ActivityTypes
                .InOrganization(request.OrganizationId)
               .Include(x => x.Project)
               .AsNoTracking()
               .AsSplitQuery()
               .FirstOrDefaultAsync(x => x.Id == request.ActivityId, cancellationToken);

            if (activityType is null)
            {
                throw new Exception();
            }

            return activityType.ToDto();
        }
    }
}