
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.TimeReport.Application.Common.Interfaces;
using YourBrand.TimeReport.Application.Common.Models;

namespace YourBrand.TimeReport.Application.Activities.Queries;

public record GetActivityStatisticsSummaryQuery(string OrganizationId, string ActivityId) : IRequest<StatisticsSummary>
{
    public class GetStatisticsSummaryQueryHandler(ITimeReportContext context) : IRequestHandler<GetActivityStatisticsSummaryQuery, StatisticsSummary>
    {
        public async Task<StatisticsSummary> Handle(GetActivityStatisticsSummaryQuery request, CancellationToken cancellationToken)
        {
            var activity = await context.Activities
               .InOrganization(request.OrganizationId)
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