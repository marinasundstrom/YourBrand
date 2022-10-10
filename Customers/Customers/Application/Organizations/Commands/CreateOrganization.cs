
using YourBrand.Customers.Domain;

using MediatR;
using YourBrand.Customers.Application.Organizations;

namespace YourBrand.Customers.Application.Organizations.Commands;

public record CreateOrganization(string Name, string OrgNo, string? Phone, string? PhoneMobile, string? Email) : IRequest<OrganizationDto>
{
    public class Handler : IRequestHandler<CreateOrganization, OrganizationDto>
    {
        private readonly ICustomersContext _context;

        public Handler(ICustomersContext context)
        {
            _context = context;
        }

        public async Task<OrganizationDto> Handle(CreateOrganization request, CancellationToken cancellationToken)
        {
            var organization = new Domain.Entities.Organization(request.Name, request.OrgNo, string.Empty);
            organization.Phone = request.Phone;
            organization.PhoneMobile = request.PhoneMobile!;
            organization.Email = request.Email!;

            _context.Organizations.Add(organization);

            await _context.SaveChangesAsync(cancellationToken);

            return organization.ToDto();
        }
    }
}
