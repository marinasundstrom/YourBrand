
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.TimeReport.Application.Common.Interfaces;

namespace YourBrand.TimeReport.Application.Users.Absence.Commands;

public record ReportAbsenceCommand(string OrganizationId, string ProjectId, DateTime Date, decimal Amount, string? Description) : IRequest<AbsenceDto>
{
    public class ReportAbsenceCommandHandler(ITimeReportContext context) : IRequestHandler<ReportAbsenceCommand, AbsenceDto>
    {
        public async Task<AbsenceDto> Handle(ReportAbsenceCommand request, CancellationToken cancellationToken)
        {
            var project = await context.Projects
               .AsSplitQuery()
               .FirstOrDefaultAsync(x => x.Id == request.ProjectId, cancellationToken);

            if (project is null)
            {
                throw new Exception();
            }

            var absence = new Domain.Entities.Absence()
            {
                Date = DateOnly.FromDateTime(request.Date),
                Note = request.Description,
                Project = project
            };

            context.Absence.Add(absence);

            await context.SaveChangesAsync(cancellationToken);

            return absence.ToDto();
        }
    }
}