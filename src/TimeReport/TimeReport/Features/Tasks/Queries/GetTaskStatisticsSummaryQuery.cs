
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.TimeReport.Features.Common.Interfaces;
using YourBrand.TimeReport.Features.Common.Models;

namespace YourBrand.TimeReport.Features.Tasks.Queries;

public record GetTaskStatisticsSummaryQuery(string OrganizationId, string TaskId) : IRequest<StatisticsSummary>
{
    public class GetStatisticsSummaryQueryHandler(ITimeReportContext context) : IRequestHandler<GetTaskStatisticsSummaryQuery, StatisticsSummary>
    {
        public async Task<StatisticsSummary> Handle(GetTaskStatisticsSummaryQuery request, CancellationToken cancellationToken)
        {
            var task = await context.Tasks
               .InOrganization(request.OrganizationId)
               .Include(x => x.Entries)
               .ThenInclude(x => x.User)
               .AsSplitQuery()
               .AsNoTracking()
               .FirstOrDefaultAsync(x => x.Id == request.TaskId, cancellationToken);

            if (task is null)
            {
                throw new Exception();
            }

            var totalHours = task.Entries
                .Sum(p => p.Hours.GetValueOrDefault());

            var totalUsers = task.Entries
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