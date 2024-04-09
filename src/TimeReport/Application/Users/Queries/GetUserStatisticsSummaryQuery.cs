
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.TimeReport.Application.Common.Interfaces;
using YourBrand.TimeReport.Application.Common.Models;
using YourBrand.TimeReport.Domain.Exceptions;

namespace YourBrand.TimeReport.Application.Users.Queries;

public record GetUserStatisticsSummaryQuery(string UserId) : IRequest<StatisticsSummary>
{
    public class GetUserStatisticsSummaryQueryHandler(ITimeReportContext context) : IRequestHandler<GetUserStatisticsSummaryQuery, StatisticsSummary>
    {
        public async Task<StatisticsSummary> Handle(GetUserStatisticsSummaryQuery request, CancellationToken cancellationToken)
        {
            var user = await context.Users
                       .AsNoTracking()
                       .AsSplitQuery()
                       .FirstOrDefaultAsync(x => x.Id == request.UserId, cancellationToken);

            if (user is null)
            {
                throw new UserNotFoundException(request.UserId);
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

            return new StatisticsSummary(new StatisticsSummaryEntry[]
            {
                new ("Projects", totalProjects),
                new ("Hours", totalHours)
            });
        }
    }
}