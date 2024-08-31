﻿
using MediatR;

using Microsoft.EntityFrameworkCore;

using OfficeOpenXml;

using YourBrand.TimeReport.Application.Common.Interfaces;
using YourBrand.TimeReport.Domain;
using YourBrand.TimeReport.Domain.Entities;

namespace YourBrand.TimeReport.Application.Reports.Queries;

public record CreateReportCommand(string OrganizationId, string[] ProjectIds, string? UserId, DateTime StartDate, DateTime EndDate, int[] Statuses, ReportMode Mode) : IRequest<Stream?>
{
    public class CreateReportCommandHandler(ITimeReportContext context) : IRequestHandler<CreateReportCommand, Stream?>
    {
        public async Task<Stream?> Handle(CreateReportCommand request, CancellationToken cancellationToken)
        {
            DateOnly startDate2 = DateOnly.FromDateTime(request.StartDate);
            DateOnly endDate2 = DateOnly.FromDateTime(request.EndDate);

            var query = context.Entries
                .Include(p => p.TimeSheet)
                .ThenInclude(p => p.User)
                .Include(p => p.Project)
                .Include(p => p.Activity)
                .Where(p => request.ProjectIds.Any(x => x == p.Project.Id))
                .Where(p => p.Date >= startDate2 && p.Date <= endDate2)
                .AsSplitQuery();

            query = query.Where(x => request.Statuses.Any(x2 => x2 == (int)x.TimeSheet.Status));

            if (request.UserId is not null)
            {
                query = query.Where(x => x.TimeSheet.User.Id == request.UserId);
            }

            var entries = await query.ToListAsync(cancellationToken);

            if (!entries.Any())
            {
                return Stream.Null;
            }

            int row = 1;

            using (var package = new ExcelPackage())
            {
                IEnumerable<IGrouping<string, Entry>> entryGroups = default!;

                if (request.Mode == ReportMode.User)
                {
                    entryGroups = entries.GroupBy(x => x.UserId.ToString());
                }
                else if (request.Mode == ReportMode.Project)
                {
                    entryGroups = entries.GroupBy(x => x.Project.Id);
                }

                foreach (var entryGroup in entryGroups)
                {
                    string worksheetName = default!;

                    if (request.Mode == ReportMode.User)
                    {
                        worksheetName = $"{entryGroup.First().User.FirstName} {entryGroup.First().User.LastName}";
                    }
                    else if (request.Mode == ReportMode.Project)
                    {
                        worksheetName = entryGroup.First().Project.Name;
                    }

                    var worksheet = package.Workbook.Worksheets.Add(worksheetName);

                    var projectGroups = entryGroup.GroupBy(x => x.Project);

                    row = 1;

                    foreach (var project in projectGroups)
                    {
                        int headerRow = row;

                        if (request.Mode == ReportMode.User)
                        {
                            worksheet.Cells[row++, 1]
                                .LoadFromCollection(new[] { new { Project = project.Key.Name } });

                            headerRow = row - 1;
                        }

                        var activityGroups = project.GroupBy(x => x.Activity);

                        foreach (var activityGroup in activityGroups)
                        {
                            var data = activityGroup
                                .OrderBy(e => e.Date)
                                .Select(e => new { e.Date, User = e.TimeSheet.User.GetDisplayName(), Project = e.Project.Name, Activity = e.Activity.Name, e.Hours, e.Description, Status = e.Status.ToString(), e.TimeSheet.Id });

                            worksheet.Cells[row, 1]
                                .LoadFromCollection(data);

                            row += data.Count();

                            if (request.Mode == ReportMode.User)
                            {
                                worksheet.Cells[headerRow, 5]
                                    .Value = data.Sum(e => e.Hours.GetValueOrDefault());
                            }
                            else
                            {
                                worksheet.Cells[row++, 5]
                                    .Value = data.Sum(e => e.Hours.GetValueOrDefault());
                            }
                        }

                        row++;
                    }
                }

                Stream stream = new MemoryStream(package.GetAsByteArray());

                stream.Seek(0, SeekOrigin.Begin);
                return stream;
            }
        }
    }
}