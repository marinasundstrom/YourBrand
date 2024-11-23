
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.TimeReport.Application.Common.Interfaces;
using YourBrand.TimeReport.Application.Common.Models;

namespace YourBrand.TimeReport.Application.Projects.Queries;

public record GetProjectStatisticsSummaryQuery(string OrganizationId) : IRequest<Result<StatisticsSummary>>
{
    public class GetProjectStatisticsSummaryHandler(ITimeReportContext context) : IRequestHandler<GetProjectStatisticsSummaryQuery, Result<StatisticsSummary>>
    {
        public async Task<Result<StatisticsSummary>> Handle(GetProjectStatisticsSummaryQuery request, CancellationToken cancellationToken)
        {
            var entries = await context.Entries
                .InOrganization(request.OrganizationId)
                .CountAsync();

            var totalProjects = await context.Projects
                .InOrganization(request.OrganizationId)
               .CountAsync();

            var totalUsers = await context.Users
                .CountAsync();

            var totalHours = await context.Entries
                .InOrganization(request.OrganizationId)
                .SumAsync(p => p.Hours.GetValueOrDefault());

            var revenue = await context.Entries
                .InOrganization(request.OrganizationId)
                .Where(e => e.Activity.HourlyRate.GetValueOrDefault() > 0)
                .SumAsync(e => e.Activity.HourlyRate.GetValueOrDefault() * (decimal)e.Hours.GetValueOrDefault());

            var expenses = await context.Entries
                .InOrganization(request.OrganizationId)
                 .Where(e => e.Activity.HourlyRate.GetValueOrDefault() < 0)
                 .SumAsync(e => e.Activity.HourlyRate.GetValueOrDefault() * (decimal)e.Hours.GetValueOrDefault());

            expenses -= await context.Expenses
                 .InOrganization(request.OrganizationId)
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