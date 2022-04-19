
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.TimeReport.Application.Common.Interfaces;
using YourBrand.TimeReport.Application.Projects;

using static YourBrand.TimeReport.Application.Users.Absence.AbsenceHelpers;

namespace YourBrand.TimeReport.Application.Users.Absence.Queries;

public record GetAbsenceQuery(string AbsenceId) : IRequest<AbsenceDto>
{
    public class GetAbsenceQueryHandler : IRequestHandler<GetAbsenceQuery, AbsenceDto>
    {
        private readonly ITimeReportContext _context;

        public GetAbsenceQueryHandler(ITimeReportContext context)
        {
            _context = context;
        }

        public async Task<AbsenceDto> Handle(GetAbsenceQuery request, CancellationToken cancellationToken)
        {
            var absence = await _context.Absence
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