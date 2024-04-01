
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.TimeReport.Application.Common.Interfaces;

namespace YourBrand.TimeReport.Application.Activities.ActivityTypes.Queries;

public record GetActivityTypeQuery(string ActivityId) : IRequest<ActivityTypeDto>
{
    public class GetActivityQueryHandler : IRequestHandler<GetActivityTypeQuery, ActivityTypeDto>
    {
        private readonly ITimeReportContext _context;

        public GetActivityQueryHandler(ITimeReportContext context)
        {
            _context = context;
        }

        public async Task<ActivityTypeDto> Handle(GetActivityTypeQuery request, CancellationToken cancellationToken)
        {
            var activityType = await _context.ActivityTypes
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