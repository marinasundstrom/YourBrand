
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.TimeReport.Application.Common.Interfaces;

namespace YourBrand.TimeReport.Application.Users.Absence.Commands;

public record DeleteAbsenceCommand(string AbsenceId) : IRequest
{
    public class DeleteAbsenceCommandHandler : IRequestHandler<DeleteAbsenceCommand>
    {
        private readonly ITimeReportContext _context;

        public DeleteAbsenceCommandHandler(ITimeReportContext context)
        {
            _context = context;
        }

        public async Task Handle(DeleteAbsenceCommand request, CancellationToken cancellationToken)
        {
            var absence = await _context.Absence
                .AsSplitQuery()
                .FirstOrDefaultAsync(x => x.Id == request.AbsenceId, cancellationToken);

            if (absence is null)
            {
                throw new Exception();
            }

            _context.Absence.Remove(absence);

            await _context.SaveChangesAsync(cancellationToken);

        }
    }
}