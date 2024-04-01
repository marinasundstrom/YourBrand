
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.TimeReport.Application.Common.Interfaces;

namespace YourBrand.TimeReport.Application.Users.Absence.Commands;

public record ReportAbsenceCommand(string ProjectId, DateTime Date, decimal Amount, string? Description) : IRequest<AbsenceDto>
{
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