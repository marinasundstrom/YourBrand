
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.TimeReport.Application.Common.Interfaces;

namespace YourBrand.TimeReport.Application.Users.Absence.Queries;

public record GetAbsenceQuery(string OrganizationId, string AbsenceId) : IRequest<AbsenceDto>
{
    public class GetAbsenceQueryHandler(ITimeReportContext context) : IRequestHandler<GetAbsenceQuery, AbsenceDto>
    {
        public async Task<AbsenceDto> Handle(GetAbsenceQuery request, CancellationToken cancellationToken)
        {
            var absence = await context.Absence
               .Include(x => x.Project)
               .AsNoTracking()
               .AsSplitQuery()
               .FirstOrDefaultAsync(x => x.Id == request.AbsenceId, cancellationToken);

            if (absence is null)
            {
                throw new Exception();
            }

            return absence.ToDto();
        }
    }
}