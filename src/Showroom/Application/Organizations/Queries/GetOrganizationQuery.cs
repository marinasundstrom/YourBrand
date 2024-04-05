using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;
using YourBrand.Showroom.Application.Common.Interfaces;

namespace YourBrand.Showroom.Application.Organizations.Queries;

public record GetOrganizationQuery(string Id) : IRequest<OrganizationDto?>
{
    class GetOrganizationQueryHandler : IRequestHandler<GetOrganizationQuery, OrganizationDto?>
    {
        private readonly IShowroomContext _context;
        private readonly IUserContext userContext;

        public GetOrganizationQueryHandler(
            IShowroomContext context,
            IUserContext userContext)
        {
            _context = context;
            this.userContext = userContext;
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