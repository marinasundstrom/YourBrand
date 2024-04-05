using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;
using YourBrand.Inventory.Domain;

namespace YourBrand.Inventory.Application.Sites.Queries;

public record GetSiteQuery(string Id) : IRequest<SiteDto?>
{
    class GetSiteQueryHandler : IRequestHandler<GetSiteQuery, SiteDto?>
    {
        private readonly IInventoryContext _context;
        private readonly IUserContext userContext;

        public GetSiteQueryHandler(
            IInventoryContext context,
            IUserContext userContext)
        {
            _context = context;
            this.userContext = userContext;
        }

        public async Task<SiteDto?> Handle(GetSiteQuery request, CancellationToken cancellationToken)
        {
            var site = await _context
               .Sites
               .AsNoTracking()
               .FirstAsync(c => c.Id == request.Id);

            if (site is null)
            {
                return null;
            }

            return site.ToDto();
        }
    }
}