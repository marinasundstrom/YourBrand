
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.TimeReport.Application.Common.Interfaces;

namespace YourBrand.TimeReport.Application.Activities.Queries;

public record GetActivityQuery(string ActivityId) : IRequest<ActivityDto>
{
    public class GetActivityQueryHandler : IRequestHandler<GetActivityQuery, ActivityDto>
    {
        private readonly ITimeReportContext _context;

        public GetActivityQueryHandler(ITimeReportContext context)
        {
            _context = context;
        }

        public async Task<ActivityDto> Handle(GetActivityQuery request, CancellationToken cancellationToken)
        {
            var activity = await _context.Activities
               .Include(x => x.ActivityType)
               .Include(x => x.Project)
               .ThenInclude(x => x.Organization)
               .AsNoTracking()
               .AsSplitQuery()
               .FirstOrDefaultAsync(x => x.Id == request.ActivityId, cancellationToken);

            if (activity is null)
            {
                throw new Exception();
            }

            return activity.ToDto();
        }
    }
}