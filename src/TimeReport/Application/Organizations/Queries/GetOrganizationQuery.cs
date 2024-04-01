using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;
using YourBrand.TimeReport.Application.Common.Interfaces;

namespace YourBrand.TimeReport.Application.Organizations
.Queries;

public record GetOrganizationQuery(string Id) : IRequest<OrganizationDto?>
{
    class GetOrganizationQueryHandler : IRequestHandler<GetOrganizationQuery, OrganizationDto?>
    {
        private readonly ITimeReportContext _context;
        private readonly ICurrentUserService currentUserService;

        public GetOrganizationQueryHandler(
            ITimeReportContext context,
            ICurrentUserService currentUserService)
        {
            _context = context;
            this.currentUserService = currentUserService;
        }

        public async Task<OrganizationDto?> Handle(GetOrganizationQuery request, CancellationToken cancellationToken)
        {
            var organization = await _context
               .Organizations
               .AsNoTracking()
               .FirstAsync(c => c.Id == request.Id);

            if (organization is null)
            {
                return null;
            }

            return organization.ToDto();
        }
    }
}