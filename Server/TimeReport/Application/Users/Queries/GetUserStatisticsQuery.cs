
using MediatR;

using Microsoft.EntityFrameworkCore;

using TimeReport.Application.Common.Interfaces;
using TimeReport.Application.Common.Models;

namespace TimeReport.Application.Users.Queries;

public class GetUserStatisticsQuery : IRequest<Data>
{
    public GetUserStatisticsQuery(string userId, DateTime? from = null, DateTime? to = null)
    {
        UserId = userId;
        From = from;
        To = to;
    }

    public string UserId { get; }

    public DateTime? From { get; }

    public DateTime? To { get; }

    public class GetUserStatisticsQueryHandler : IRequestHandler<GetUserStatisticsQuery, Data>
    {
        private readonly ITimeReportContext _context;

        public GetUserStatisticsQueryHandler(ITimeReportContext context)
        {
            _context = context;
        }

        public async Task<Data> Handle(GetUserStatisticsQuery request, CancellationToken cancellationToken)
        {
            var projects = await _context.Projects
                .Include(x => x.Memberships)
                .ThenInclude(x => x.User)
                .Include(x => x.Activities)
                .ThenInclude(x => x.Entries)
                .ThenInclude(x => x.User)
                .Where(x => x.Memberships.Any(x => x.User.Id == request.UserId))
                .AsNoTracking()
                .AsSplitQuery()
                .ToListAsync(cancellationToken);

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
                        .Where(e => e.User.Id == request.UserId)
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