
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.TimeReport.Application.Common.Interfaces;
using YourBrand.TimeReport.Application.Common.Models;

namespace YourBrand.TimeReport.Application.Projects.Queries;

public record GetProjectStatisticsForProjectQuery(string OrganizationId, string ProjectId, DateTime? From = null, DateTime? To = null) : IRequest<Result<Data>>
{
    public class GetProjectStatisticsForProjectQueryHandler(ITimeReportContext context) : IRequestHandler<GetProjectStatisticsForProjectQuery, Result<Data>>
    {
        public async Task<Result<Data>> Handle(GetProjectStatisticsForProjectQuery request, CancellationToken cancellationToken)
        {
            var project = await context.Projects
                .InOrganization(request.OrganizationId)
                .Include(x => x.Tasks)
                .ThenInclude(x => x.Entries)
                .AsNoTracking()
                .AsSplitQuery()
                .FirstOrDefaultAsync(x => x.Id == request.ProjectId);

            if (project is null)
            {
                return new ProjectNotFound(request.ProjectId);
            }

            List<DateTime> months = new();

            const int monthSpan = 5;

            DateTime lastDate = request.To?.Date ?? DateTime.Now.Date;
            DateTime firstDate = request.From?.Date ?? lastDate.AddMonths(-monthSpan)!;

            for (DateTime dt = firstDate; dt <= lastDate; dt = dt.AddMonths(1))
            {
                months.Add(dt);
            }

            List<Series> series = new();

            var firstMonth = DateOnly.FromDateTime(firstDate);
            var lastMonth = DateOnly.FromDateTime(lastDate);

            foreach (var task in project.Tasks)
            {
                List<decimal> values = new();

                foreach (var month in months)
                {
                    var value = task.Entries
                        .Where(e => e.Date.Year == month.Year && e.Date.Month == month.Month)
                        .Select(x => x.Hours.GetValueOrDefault())
                        .Sum();

                    values.Add((decimal)value);
                }

                series.Add(new Series(task.Name, values));
            }

            return new Data(
                months.Select(d => d.ToString("MMM yy")).ToArray(),
                series);
        }
    }
}