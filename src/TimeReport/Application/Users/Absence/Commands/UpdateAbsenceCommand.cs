
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.TimeReport.Application.Common.Interfaces;
using YourBrand.TimeReport.Application.Projects;

using static YourBrand.TimeReport.Application.Users.Absence.AbsenceHelpers;

namespace YourBrand.TimeReport.Application.Users.Absence.Commands;

public record UpdateAbsenceCommand(string AbsenceId, DateTime Date, decimal Amount, string? Description) : IRequest<AbsenceDto>
{
    public class UpdateAbsenceCommandHandler : IRequestHandler<UpdateAbsenceCommand, AbsenceDto>
    {
        private readonly ITimeReportContext _context;

        public UpdateAbsenceCommandHandler(ITimeReportContext context)
        {
            _context = context;
        }

        public async Task<AbsenceDto> Handle(UpdateAbsenceCommand request, CancellationToken cancellationToken)
        {
            var absence = await _context.Absence
                .Include(x => x.Project)
                .AsSplitQuery()
                .FirstOrDefaultAsync(x => x.Id == request.AbsenceId, cancellationToken);

            if (absence is null)
            {
                throw new Exception();
            }

            absence.Date = DateOnly.FromDateTime(request.Date);
            absence.Note = request.Description;

            await _context.SaveChangesAsync(cancellationToken);

            return absence.ToDto();
        }
    }
}