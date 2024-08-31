
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.TimeReport.Application.Common.Interfaces;
using YourBrand.TimeReport.Application.Common.Models;

namespace YourBrand.TimeReport.Application.Projects.Queries;

public record GetProjectStatisticsSummaryQuery(string OrganizationId) : IRequest<StatisticsSummary>
{
    public class GetProjectStatisticsSummaryHandler(ITimeReportContext context) : IRequestHandler<GetProjectStatisticsSummaryQuery, StatisticsSummary>
    {
        public async Task<StatisticsSummary> Handle(GetProjectStatisticsSummaryQuery request, CancellationToken cancellationToken)
        {
            var entries = await context.Entries
                .CountAsync();

            var totalProjects = await context.Projects
               .CountAsync();

            var totalUsers = await context.Users
                .CountAsync();

            var totalHours = await context.Entries
                .SumAsync(p => p.Hours.GetValueOrDefault());

            var revenue = await context.Entries
                .Where(e => e.Activity.HourlyRate.GetValueOrDefault() > 0)
                .SumAsync(e => e.Activity.HourlyRate.GetValueOrDefault() * (decimal)e.Hours.GetValueOrDefault());

            var expenses = await context.Entries
                 .Where(e => e.Activity.HourlyRate.GetValueOrDefault() < 0)
                 .SumAsync(e => e.Activity.HourlyRate.GetValueOrDefault() * (decimal)e.Hours.GetValueOrDefault());

            expenses -= await context.Expenses
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