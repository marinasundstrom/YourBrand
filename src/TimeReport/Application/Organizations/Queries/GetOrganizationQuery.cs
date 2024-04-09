using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;
using YourBrand.TimeReport.Application.Common.Interfaces;

namespace YourBrand.TimeReport.Application.Organizations
.Queries;

public record GetOrganizationQuery(string Id) : IRequest<OrganizationDto?>
{
    class GetOrganizationQueryHandler(
        ITimeReportContext context,
        IUserContext userContext) : IRequestHandler<GetOrganizationQuery, OrganizationDto?>
    {
        public async Task<OrganizationDto?> Handle(GetOrganizationQuery request, CancellationToken cancellationToken)
        {
            var organization = await context
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