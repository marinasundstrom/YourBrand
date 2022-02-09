
using MediatR;

using Microsoft.EntityFrameworkCore;

using OfficeOpenXml;

using TimeReport.Application.Common.Interfaces;

namespace TimeReport.Application.Reports.Queries;

public class CreateReportCommand : IRequest<Stream?>
{
    public CreateReportCommand(string[] projectIds, string? userId, DateTime startDate, DateTime endDate)
    {
        ProjectIds = projectIds;
        UserId = userId;
        StartDate = startDate;
        EndDate = endDate;
    }

    public string[] ProjectIds { get; }

    public string? UserId { get; }

    public DateTime StartDate { get; }

    public DateTime EndDate { get; }

    public class CreateReportCommandHandler : IRequestHandler<CreateReportCommand, Stream?>
    {
        private readonly ITimeReportContext _context;

        public CreateReportCommandHandler(ITimeReportContext context)
        {
            _context = context;
        }

        public async Task<Stream?> Handle(CreateReportCommand request, CancellationToken cancellationToken)
        {
            DateOnly startDate2 = DateOnly.FromDateTime(request.StartDate);
            DateOnly endDate2 = DateOnly.FromDateTime(request.EndDate);

            var query = _context.Entries
                .Include(p => p.TimeSheet)
                .ThenInclude(p => p.User)
                .Include(p => p.Project)
                .Include(p => p.Activity)
                .Where(p => request.ProjectIds.Any(x => x == p.Project.Id))
                .Where(p => p.Date >= startDate2 && p.Date <= endDate2)
                .AsSplitQuery();

            if (request.UserId is not null)
            {
                query = query.Where(x => x.TimeSheet.User.Id == request.UserId);
            }

            var entries = await query.ToListAsync(cancellationToken);

            int row = 1;

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Projects");

                var projectGroups = entries.GroupBy(x => x.Project);

                foreach (var project in projectGroups)
                {
                    worksheet.Cells[row++, 1]
                          .LoadFromCollection(new[] { new { Project = project.Key.Name } });

                    int headerRow = row - 1;

                    var activityGroups = project.GroupBy(x => x.Activity);

                    foreach (var activityGroup in activityGroups)
                    {
                        var data = activityGroup
                            .OrderBy(e => e.Date)
                            .Select(e => new { e.Date, User = $" {e.TimeSheet.User.LastName}, {e.TimeSheet.User.FirstName}", Activity = e.Activity.Name, e.Hours, e.Description });

                        worksheet.Cells[row, 1]
                            .LoadFromCollection(data);

                        row += data.Count();

                        worksheet.Cells[headerRow, 4]
                            .Value = data.Sum(e => e.Hours.GetValueOrDefault());
                    }

                    row++;
                }

                Stream stream = new MemoryStream(package.GetAsByteArray());

                stream.Seek(0, SeekOrigin.Begin);
                return stream;
            }
        }
    }
}