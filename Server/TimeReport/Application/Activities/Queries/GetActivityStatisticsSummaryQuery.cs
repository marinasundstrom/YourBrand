
using MediatR;

using Microsoft.EntityFrameworkCore;

using TimeReport.Application.Common.Interfaces;
using TimeReport.Application.Common.Models;

namespace TimeReport.Application.Activities.Queries;

public class GetActivityStatisticsSummaryQuery : IRequest<StatisticsSummary>
{
    public GetActivityStatisticsSummaryQuery(string activityId)
    {
        ActivityId = activityId;
    }

    public string ActivityId { get; }

    public class GetStatisticsSummaryQueryHandler : IRequestHandler<GetActivityStatisticsSummaryQuery, StatisticsSummary>
    {
        private readonly ITimeReportContext _context;

        public GetStatisticsSummaryQueryHandler(ITimeReportContext context)
        {
            _context = context;
        }

        public async Task<StatisticsSummary> Handle(GetActivityStatisticsSummaryQuery request, CancellationToken cancellationToken)
        {
            var activity = await _context.Activities
               .Include(x => x.Entries)
               .ThenInclude(x => x.User)
               .AsSplitQuery()
               .AsNoTracking()
               .FirstOrDefaultAsync(x => x.Id == request.ActivityId, cancellationToken);

            if (activity is null)
            {
                throw new Exception();
            }

            var totalHours = activity.Entries
                .Sum(p => p.Hours.GetValueOrDefault());

            var totalUsers = activity.Entries
                .Select(p => p.User)
                .DistinctBy(p => p.Id)
                .Count();

            return new StatisticsSummary(new StatisticsSummaryEntry[]
            {
                new ("Participants", totalUsers),
                new ("Hours", totalHours)
            });
        }
    }
}