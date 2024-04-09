
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.TimeReport.Application.Common.Interfaces;

namespace YourBrand.TimeReport.Application.Users.Absence.Commands;

public record DeleteAbsenceCommand(string AbsenceId) : IRequest
{
    public class DeleteAbsenceCommandHandler(ITimeReportContext context) : IRequestHandler<DeleteAbsenceCommand>
    {
        public async Task Handle(DeleteAbsenceCommand request, CancellationToken cancellationToken)
        {
            var absence = await context.Absence
                .AsSplitQuery()
                .FirstOrDefaultAsync(x => x.Id == request.AbsenceId, cancellationToken);

            if (absence is null)
            {
                throw new Exception();
            }

            context.Absence.Remove(absence);

            await context.SaveChangesAsync(cancellationToken);

        }
    }
}