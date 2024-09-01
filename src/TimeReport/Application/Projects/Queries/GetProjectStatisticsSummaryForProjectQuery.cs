
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.TimeReport.Application.Common.Interfaces;
using YourBrand.TimeReport.Application.Common.Models;
using YourBrand.TimeReport.Domain.Exceptions;

namespace YourBrand.TimeReport.Application.Projects.Queries;

public record GetProjectStatisticsSummaryForProjectQuery(string OrganizationId, string ProjectId) : IRequest<StatisticsSummary>
{
    public class GetProjectStatisticsSummaryForQueryHandler(ITimeReportContext context) : IRequestHandler<GetProjectStatisticsSummaryForProjectQuery, StatisticsSummary>
    {
        public async Task<StatisticsSummary> Handle(GetProjectStatisticsSummaryForProjectQuery request, CancellationToken cancellationToken)
        {
            var project = await context.Projects
                .InOrganization(request.OrganizationId)
                .Include(p => p.Entries)
                .ThenInclude(x => x.User)
                .Include(p => p.Entries)
                .ThenInclude(x => x.Activity)
                .Include(p => p.Expenses)
                .AsSplitQuery()
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == request.ProjectId);

            if (project is null)
            {
                throw new ProjectNotFoundException(request.ProjectId);
            }

            var totalHours = project.Entries
                .Sum(e => e.Hours.GetValueOrDefault());

            var revenue = project.Entries
                .Where(e => e.Activity.HourlyRate.GetValueOrDefault() > 0)
                .Sum(e => e.Activity.HourlyRate.GetValueOrDefault() * (decimal)e.Hours.GetValueOrDefault());

            var expenses = project.Entries
                 .Where(e => e.Activity.HourlyRate.GetValueOrDefault() < 0)
                 .Sum(e => e.Activity.HourlyRate.GetValueOrDefault() * (decimal)e.Hours.GetValueOrDefault());

            expenses -= project.Expenses
                 .Sum(e => e.Amount);

            var totalUsers = project.Entries
                .Select(e => e.User)
                .DistinctBy(e => e.Id)
                .Count();

            return new StatisticsSummary(new StatisticsSummaryEntry[]
            {
                new ("Participants", totalUsers),
                new ("Hours", totalHours),
                new ("Revenue", null, revenue, unit: "currency"),
                new ("Expenses", null, expenses, unit: "currency")
            });
        }
    }
}