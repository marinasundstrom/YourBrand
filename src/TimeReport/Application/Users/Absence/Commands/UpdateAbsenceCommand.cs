
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.TimeReport.Application.Common.Interfaces;

namespace YourBrand.TimeReport.Application.Users.Absence.Commands;

public record UpdateAbsenceCommand(string OrganizationId, string AbsenceId, DateTime Date, decimal Amount, string? Description) : IRequest<AbsenceDto>
{
    public class UpdateAbsenceCommandHandler(ITimeReportContext context) : IRequestHandler<UpdateAbsenceCommand, AbsenceDto>
    {
        public async Task<AbsenceDto> Handle(UpdateAbsenceCommand request, CancellationToken cancellationToken)
        {
            var absence = await context.Absence
                .Include(x => x.Project)
                .AsSplitQuery()
                .FirstOrDefaultAsync(x => x.Id == request.AbsenceId, cancellationToken);

            if (absence is null)
            {
                throw new Exception();
            }

            absence.Date = DateOnly.FromDateTime(request.Date);
            absence.Note = request.Description;

            await context.SaveChangesAsync(cancellationToken);

            return absence.ToDto();
        }
    }
}