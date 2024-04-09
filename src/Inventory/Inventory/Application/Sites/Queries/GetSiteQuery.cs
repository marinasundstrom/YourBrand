using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;
using YourBrand.Inventory.Domain;

namespace YourBrand.Inventory.Application.Sites.Queries;

public record GetSiteQuery(string Id) : IRequest<SiteDto?>
{
    sealed class GetSiteQueryHandler(
        IInventoryContext context,
        IUserContext userContext) : IRequestHandler<GetSiteQuery, SiteDto?>
    {
        public async Task<SiteDto?> Handle(GetSiteQuery request, CancellationToken cancellationToken)
        {
            var site = await context
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