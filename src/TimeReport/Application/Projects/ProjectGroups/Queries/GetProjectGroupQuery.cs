
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.TimeReport.Application.Common.Interfaces;
using YourBrand.TimeReport.Application.Projects;

namespace YourBrand.TimeReport.Application.Projects.ProjectGroups.Queries;

public record GetProjectGroupQuery(string ExpenseId) : IRequest<ProjectGroupDto>
{
    public class GetExpenseQueryHandler : IRequestHandler<GetProjectGroupQuery, ProjectGroupDto>
    {
        private readonly ITimeReportContext _context;

        public GetExpenseQueryHandler(ITimeReportContext context)
        {
            _context = context;
        }

        public async Task<ProjectGroupDto> Handle(GetProjectGroupQuery request, CancellationToken cancellationToken)
        {
            var projectGroup = await _context.ProjectGroups
               .Include(x => x.Project)
               .AsNoTracking()
               .AsSplitQuery()
               .FirstOrDefaultAsync(x => x.Id == request.ExpenseId, cancellationToken);

            if (projectGroup is null)
            {
                throw new Exception();
            }

            return projectGroup.ToDto();
        }
    }
}