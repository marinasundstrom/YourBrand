
using MediatR;

using Microsoft.EntityFrameworkCore;

using TimeReport.Application.Common.Interfaces;
using TimeReport.Application.Common.Models;
using TimeReport.Domain.Exceptions;

namespace TimeReport.Application.Users.Queries;

public class GetUserStatisticsSummaryQuery : IRequest<StatisticsSummary>
{
    public GetUserStatisticsSummaryQuery(string userId)
    {
        UserId = userId;
    }

    public string UserId { get; }

    public class GetUserStatisticsSummaryQueryHandler : IRequestHandler<GetUserStatisticsSummaryQuery, StatisticsSummary>
    {
        private readonly ITimeReportContext _context;

        public GetUserStatisticsSummaryQueryHandler(ITimeReportContext context)
        {
            _context = context;
        }

        public async Task<StatisticsSummary> Handle(GetUserStatisticsSummaryQuery request, CancellationToken cancellationToken)
        {
            var user = await _context.Users
                       .AsNoTracking()
                       .AsSplitQuery()
                       .FirstOrDefaultAsync(x => x.Id == request.UserId, cancellationToken);

            if (user is null)
            {
                throw new UserNotFoundException(request.UserId);
            }

            var entries = await _context.Entries
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