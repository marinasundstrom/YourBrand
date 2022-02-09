
using MediatR;

using Microsoft.EntityFrameworkCore;

using TimeReport.Application.Common.Interfaces;
using TimeReport.Application.Common.Models;

namespace TimeReport.Application.Projects.Queries;

public class GetProjectStatisticsSummaryQuery : IRequest<StatisticsSummary>
{
    public class GetProjectStatisticsSummaryHandler : IRequestHandler<GetProjectStatisticsSummaryQuery, StatisticsSummary>
    {
        private readonly ITimeReportContext _context;

        public GetProjectStatisticsSummaryHandler(ITimeReportContext context)
        {
            _context = context;
        }

        public async Task<StatisticsSummary> Handle(GetProjectStatisticsSummaryQuery request, CancellationToken cancellationToken)
        {
            var entries = await _context.Entries
                .CountAsync();

            var totalProjects = await _context.Projects
               .CountAsync();

            var totalUsers = await _context.Users
                .CountAsync();

            var totalHours = await _context.Entries
                .SumAsync(p => p.Hours.GetValueOrDefault());

            var revenue = await _context.Entries
                .Where(e => e.Activity.HourlyRate.GetValueOrDefault() > 0)
                .SumAsync(e => e.Activity.HourlyRate.GetValueOrDefault() * (decimal)e.Hours.GetValueOrDefault());

            var expenses = await _context.Entries
                 .Where(e => e.Activity.HourlyRate.GetValueOrDefault() < 0)
                 .SumAsync(e => e.Activity.HourlyRate.GetValueOrDefault() * (decimal)e.Hours.GetValueOrDefault());

            expenses -= await _context.Expenses
                 .SumAsync(e => e.Amount);

            return new StatisticsSummary(new StatisticsSummaryEntry[]
            {
                new ("Projects", totalProjects),
                new ("Users", totalUsers),
                new ("Hours", totalHours),
                new ("Revenue", null, revenue,  unit: "currency"),
                new ("Expenses", null, expenses, unit: "currency")
            });
        }
    }
}