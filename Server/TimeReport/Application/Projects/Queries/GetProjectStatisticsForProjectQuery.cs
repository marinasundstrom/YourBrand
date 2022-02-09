
using MediatR;

using Microsoft.EntityFrameworkCore;

using TimeReport.Application.Common.Interfaces;
using TimeReport.Application.Common.Models;
using TimeReport.Domain.Exceptions;

namespace TimeReport.Application.Projects.Queries;

public class GetProjectStatisticsForProjectQuery : IRequest<Data>
{
    public GetProjectStatisticsForProjectQuery(string projectId, DateTime? from = null, DateTime? to = null)
    {
        ProjectId = projectId;
        From = from;
        To = to;
    }

    public string ProjectId { get; }

    public DateTime? From { get; }

    public DateTime? To { get; }

    public class GetProjectStatisticsForProjectQueryHandler : IRequestHandler<GetProjectStatisticsForProjectQuery, Data>
    {
        private readonly ITimeReportContext _context;

        public GetProjectStatisticsForProjectQueryHandler(ITimeReportContext context)
        {
            _context = context;
        }

        public async Task<Data> Handle(GetProjectStatisticsForProjectQuery request, CancellationToken cancellationToken)
        {
            var project = await _context.Projects
                .Include(x => x.Activities)
                .ThenInclude(x => x.Entries)
                .AsNoTracking()
                .AsSplitQuery()
                .FirstOrDefaultAsync(x => x.Id == request.ProjectId);

            if (project is null)
            {
                throw new ProjectNotFoundException(request.ProjectId);
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

            foreach (var activity in project.Activities)
            {
                List<decimal> values = new();

                foreach (var month in months)
                {
                    var value = activity.Entries
                        .Where(e => e.Date.Year == month.Year && e.Date.Month == month.Month)
                        .Select(x => x.Hours.GetValueOrDefault())
                        .Sum();

                    values.Add((decimal)value);
                }

                series.Add(new Series(activity.Name, values));
            }

            return new Data(
                months.Select(d => d.ToString("MMM yy")).ToArray(),
                series);
        }
    }
}