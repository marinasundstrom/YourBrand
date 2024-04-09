using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Customers.Domain;

namespace YourBrand.Customers.Application.Organizations.Queries;

public record GetOrganization(int Id) : IRequest<OrganizationDto?>
{
    public class Handler(ICustomersContext context) : IRequestHandler<GetOrganization, OrganizationDto?>
    {
        public async Task<OrganizationDto?> Handle(GetOrganization request, CancellationToken cancellationToken)
        {

            var organization = await context.Organizations
                .Include(i => i.Addresses)
                .AsSplitQuery()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            return organization is null
                ? null
                : organization.ToDto();
        }
    }
}