﻿
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.TimeReport.Application.Common.Interfaces;
using YourBrand.TimeReport.Application.Common.Models;

namespace YourBrand.TimeReport.Application.Projects.Queries;

public record GetProjectStatisticsQuery(string OrganizationId, DateTime? From = null, DateTime? To = null) : IRequest<Result<Data>>
{
    public class GetProjectStatisticsQueryHandler(ITimeReportContext context) : IRequestHandler<GetProjectStatisticsQuery, Result<Data>>
    {
        public async Task<Result<Data>> Handle(GetProjectStatisticsQuery request, CancellationToken cancellationToken)
        {
            var projects = await context.Projects
                .InOrganization(request.OrganizationId)
                        .Include(x => x.Tasks)
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
                    var value = project.Tasks.SelectMany(a => a.Entries)
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