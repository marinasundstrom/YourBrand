
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.TimeReport.Application.Common.Interfaces;
using YourBrand.TimeReport.Application.Common.Models;

namespace YourBrand.TimeReport.Application.Projects.Queries;

public record GetProjectStatisticsSummaryForProjectQuery(string OrganizationId, string ProjectId) : IRequest<Result<StatisticsSummary>>
{
    public class GetProjectStatisticsSummaryForQueryHandler(ITimeReportContext context) : IRequestHandler<GetProjectStatisticsSummaryForProjectQuery, Result<StatisticsSummary>>
    {
        public async Task<Result<StatisticsSummary>> Handle(GetProjectStatisticsSummaryForProjectQuery request, CancellationToken cancellationToken)
        {
            var project = await context.Projects
                .InOrganization(request.OrganizationId)
                .Include(p => p.Entries)
                .ThenInclude(x => x.User)
                .Include(p => p.Entries)
                .ThenInclude(x => x.Task)
                .Include(p => p.Expenses)
                .AsSplitQuery()
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == request.ProjectId);

            if (project is null)
            {
                return new ProjectNotFound(request.ProjectId);
            }

            var totalHours = project.Entries
                .Sum(e => e.Hours.GetValueOrDefault());

            var revenue = project.Entries
                .Where(e => e.Task.HourlyRate.GetValueOrDefault() > 0)
                .Sum(e => e.Task.HourlyRate.GetValueOrDefault() * (decimal)e.Hours.GetValueOrDefault());

            var expenses = project.Entries
                 .Where(e => e.Task.HourlyRate.GetValueOrDefault() < 0)
                 .Sum(e => e.Task.HourlyRate.GetValueOrDefault() * (decimal)e.Hours.GetValueOrDefault());

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