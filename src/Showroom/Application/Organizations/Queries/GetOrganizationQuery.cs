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
        private readonly ICurrentUserService currentUserService;

        public GetOrganizationQueryHandler(
            IShowroomContext context,
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