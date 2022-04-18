
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.TimeReport.Application.Common.Interfaces;
using YourBrand.TimeReport.Application.Projects;
using YourBrand.TimeReport.Domain.Entities;

using static YourBrand.TimeReport.Application.Users.Absence.AbsenceHelpers;

namespace YourBrand.TimeReport.Application.Users.Absence.Commands;

public class ReportAbsenceCommand : IRequest<AbsenceDto>
{
    public ReportAbsenceCommand(string projectId, DateTime date, decimal amount, string? description)
    {
        ProjectId = projectId;
        Date = date;
        Amount = amount;
        Description = description;
    }

    public string ProjectId { get; }

    public DateTime Date { get; }

    public decimal Amount { get; }

    public string? Description { get; }

    public class ReportAbsenceCommandHandler : IRequestHandler<ReportAbsenceCommand, AbsenceDto>
    {
        private readonly ITimeReportContext _context;

        public ReportAbsenceCommandHandler(ITimeReportContext context)
        {
            _context = context;
        }

        public async Task<AbsenceDto> Handle(ReportAbsenceCommand request, CancellationToken cancellationToken)
        {
            var project = await _context.Projects
               .AsSplitQuery()
               .FirstOrDefaultAsync(x => x.Id == request.ProjectId, cancellationToken);

            if (project is null)
            {
                throw new Exception();
            }

            var absence = new Domain.Entities.Absence
            {
                Id = Guid.NewGuid().ToString(),
                Date = DateOnly.FromDateTime(request.Date),
                Note = request.Description,
                Project = project
            };

            _context.Absence.Add(absence);

            await _context.SaveChangesAsync(cancellationToken);

            return absence.ToDto();
        }
    }
}