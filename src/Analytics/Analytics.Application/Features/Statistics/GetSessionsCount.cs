using MediatR;
using Microsoft.EntityFrameworkCore;

namespace YourBrand.Analytics.Application.Features.Statistics;

public record GetSessionsCount(DateTime? From = null, DateTime? To = null, bool DistinctByClient = false) : IRequest<Data>
{
    public class Handler : IRequestHandler<GetSessionsCount, Data>
    {
        private readonly IApplicationDbContext context;

        public Handler(IApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task<Data> Handle(GetSessionsCount request, CancellationToken cancellationToken)
        {
            var sessions = context.Sessions
                       .Include(x => x.Client)
                       .OrderByDescending(x => x.Started)
                       .AsNoTracking()
                       .AsSplitQuery();

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

            List<decimal> values = new();

            foreach (var month in months)
            {
                int value = 0;

                if (request.DistinctByClient)
                {
                    value = await sessions
                        .Where(e => e.Started.Year == month.Year && e.Started.Month == month.Month)
                        .GroupBy(x => x.Client).Select(x => x.FirstOrDefault())
                        .CountAsync();

                }
                else
                {
                    value = await sessions
                        .Where(e => e.Started.Year == month.Year && e.Started.Month == month.Month)
                        .CountAsync();
                }

                values.Add((decimal)value);
            }

            series.Add(new Series("Sessions", values));

            return new Data(
                months.Select(d => d.ToString("MMM yy")).ToArray(),
                series);
        }
    }
}
