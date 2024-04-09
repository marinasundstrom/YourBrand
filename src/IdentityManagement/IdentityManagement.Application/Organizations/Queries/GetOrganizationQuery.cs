
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.IdentityManagement.Application.Common.Interfaces;

namespace YourBrand.IdentityManagement.Application.Organizations.Queries;

public record GetOrganizationQuery(string OrganizationId) : IRequest<OrganizationDto>
{
    public class GetOrganizationQueryHandler(IApplicationDbContext context) : IRequestHandler<GetOrganizationQuery, OrganizationDto>
    {
        public async Task<OrganizationDto> Handle(GetOrganizationQuery request, CancellationToken cancellationToken)
        {
            var organization = await context.Organizations
                .AsNoTracking()
                .AsSplitQuery()
                .FirstOrDefaultAsync(x => x.Id == request.OrganizationId, cancellationToken);

            if (organization is null)
            {
                return null!;
            }

            return organization.ToDto();
        }
    }
}