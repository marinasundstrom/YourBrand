
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.TimeReport.Application.Common.Interfaces;
using YourBrand.TimeReport.Application.Common.Models;

namespace YourBrand.TimeReport.Application.Users.Queries;

public record GetUserStatisticsSummaryQuery(string UserId) : IRequest<Result<StatisticsSummary>>
{
    public class GetUserStatisticsSummaryQueryHandler(ITimeReportContext context) : IRequestHandler<GetUserStatisticsSummaryQuery, Result<StatisticsSummary>>
    {
        [Throws(typeof(OperationCanceledException))]
        [Throws(typeof(OverflowException))]
        public async Task<Result<StatisticsSummary>> Handle(GetUserStatisticsSummaryQuery request, CancellationToken cancellationToken)
        {
            var user = await context.Users
                       .AsNoTracking()
                       .AsSplitQuery()
                       .FirstOrDefaultAsync(x => x.Id == request.UserId, cancellationToken);

            if (user is null)
            {
               return new UserNotFound(request.UserId);
            }

            var entries = await context.Entries
                .Include(x => x.Project)
                .Where(x => x.User.Id == request.UserId)
                .AsSplitQuery()
                .AsNoTracking()
                .ToArrayAsync(cancellationToken);

            var totalHours = entries
                .Sum(p => p.Hours.GetValueOrDefault());

            var totalProjects = entries
                .Select(p => p.Project)
                .DistinctBy(p => p.Id)
                .Count();

            return new StatisticsSummary(
            [
                new ("Projects", totalProjects),
                new ("Hours", totalHours)
            ]);
        }
    }
}