
using MediatR;

using Microsoft.EntityFrameworkCore;

using TimeReport.Application.Common.Interfaces;
using TimeReport.Application.Common.Models;

namespace TimeReport.Application.Projects.Queries;

public class GetProjectStatisticsQuery : IRequest<Data>
{
    public GetProjectStatisticsQuery(DateTime? from = null, DateTime? to = null)
    {
        From = from;
        To = to;
    }

    public DateTime? From { get; }

    public DateTime? To { get; }

    public class GetProjectStatisticsQueryHandler : IRequestHandler<GetProjectStatisticsQuery, Data>
    {
        private readonly ITimeReportContext _context;

        public GetProjectStatisticsQueryHandler(ITimeReportContext context)
        {
            _context = context;
        }

        public async Task<Data> Handle(GetProjectStatisticsQuery request, CancellationToken cancellationToken)
        {
            var projects = await _context.Projects
                        .Include(x => x.Activities)
                        .ThenInclude(x => x.Entries)
                        .AsNoTracking()
                        .AsSplitQuery()
                        .ToListAsync();

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

            foreach (var project in projects)
            {
                List<decimal> values = new();

                foreach (var month in months)
                {
                    var value = project.Activities.SelectMany(a => a.Entries)
                        .Where(e => e.Date.Year == month.Year && e.Date.Month == month.Month)
                        .Select(x => x.Hours.GetValueOrDefault())
                        .Sum();

                    values.Add((decimal)value);
                }

                series.Add(new Series(project.Name, values));
            }

            return new Data(
                months.Select(d => d.ToString("MMM yy")).ToArray(),
                series);
        }
    }
}