
using MediatR;

using Microsoft.EntityFrameworkCore;

using TimeReport.Application.Common.Interfaces;
using TimeReport.Application.Common.Models;
using TimeReport.Domain.Exceptions;

namespace TimeReport.Application.Projects.Queries;

public class GetProjectStatisticsSummaryForProjectQuery : IRequest<StatisticsSummary>
{
    public GetProjectStatisticsSummaryForProjectQuery(string projectId)
    {
        ProjectId = projectId;
    }

    public string ProjectId { get; }

    public class GetProjectStatisticsSummaryForQueryHandler : IRequestHandler<GetProjectStatisticsSummaryForProjectQuery, StatisticsSummary>
    {
        private readonly ITimeReportContext _context;

        public GetProjectStatisticsSummaryForQueryHandler(ITimeReportContext context)
        {
            _context = context;
        }

        public async Task<StatisticsSummary> Handle(GetProjectStatisticsSummaryForProjectQuery request, CancellationToken cancellationToken)
        {
            var project = await _context.Projects
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